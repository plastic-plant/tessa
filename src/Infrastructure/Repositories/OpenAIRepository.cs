using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tessa.Application.Enums;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Application.Models.ProviderConfigs;

namespace Tessa.Infrastructure.Repositories;

/// <summary>
/// https://platform.openai.com/docs/api-reference
/// </summary>
public class OpenAIRepository: IOpenAIRepository
{
	private readonly ILogger<OpenAIRepository> _logger;
	private readonly IServiceProvider _services;
	private readonly ISettingsService _settings;
	private readonly ProviderConfigOpenAI _config;

	public OpenAIRepository(ILogger<OpenAIRepository> logger, IServiceProvider services, ISettingsService settings)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		_config = _settings.Settings.GetSelectedProviderConfiguration() as ProviderConfigOpenAI ?? throw new ArgumentNullException(nameof(_config));
	}

	public async Task<(bool ready, string? error)> IsReadyAsync()
	{
		try
		{
			var model = await GetAvailableMatchingModelAsync();
			return (model != null, null);
		}
		catch (Exception ex)
		{
			return (false, ex.Message);
		}
	}

	public async Task<FileSummary> ProcessAsync(FileSummary file)
	{
		using var httpClient = new HttpClient();
		if (!string.IsNullOrWhiteSpace(_config.ApiKey))
		{
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);
		}

		var content = File.ReadAllText(file.FilePathResultOcr);
		var parts = SplitTextInParts(content, _settings.Settings.Ocr.MaxPrompt);
		using var response = new StringWriter();
		foreach (var part in parts)
		{
			try
			{
				var request = new OpenAICompletionsRequest()
				{
					Model = _config.Model!,
					Messages = new List<OpenAIMessage>
					{
						new OpenAIMessage { Role = OpenAICompletionRole.User, Content = $"{_settings.Settings.Ocr.CleanupPrompt!}\n```{part}```" }
					},
					Temperature = _config.Temperature,
					MaxTokens = _config.MaxTokens,
					Stream = false
				};

				var requestJson = JsonSerializer.Serialize(request, _serializerOptions);
				var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

				var responseContent = await httpClient.PostAsync($"{_config.ApiHostUrl}/v1/chat/completions", requestContent);
				if (!responseContent.IsSuccessStatusCode)
				{
					throw new Exception($"POST request failed with status code {responseContent.StatusCode}");
				}

				var json = await responseContent.Content.ReadAsStringAsync();
				if (string.IsNullOrWhiteSpace(json))
				{
					throw new Exception("No data returned from API");
				}

				var responseModel = JsonSerializer.Deserialize<OpenAICompletionsResponse>(json, _serializerOptions);
				var optimizedText = responseModel?.Choices?.FirstOrDefault()?.Message?.Content;
				if (!string.IsNullOrWhiteSpace(optimizedText))
				{
					response.Write($" {optimizedText}");
				}
			}
			catch (Exception ex)
			{
				file.OcrProcessingStatus = OcrProcessingStatus.Failed;
				file.Errors.Add($"An error occurred: {ex.Message}");
			}
		}

		file.FilePathResultLlm = Path.Combine(_settings.Settings.Ocr.OutputPath, $"{file.FileNameWithoutExtension!}.llm.txt");
		File.WriteAllText(file.FilePathResultLlm, response.ToString());

		return file;
	}

	internal async Task<string?> GetAvailableMatchingModelAsync()
	{
		var models = await GetModelsAsync();
		var model = models?.Data?.Count switch
		{
			null => throw new Exception("Empty response or API failure"),
			0 => throw new Exception("No models found"),
			1 => models.Data.First().Id, // TheBloke/phi-2-GGUF/phi-2.Q4_K_S.gguf
			_ => models.Data.FirstOrDefault(model => model.Id!.Contains(_config.Model!, StringComparison.InvariantCultureIgnoreCase))?.Id ?? throw new Exception($"Model {_config.Name} could not be matched. Provider offers: {string.Join(", ", models.Data.Select(model => model.Id))}") // phi-2.Q4_K_S
		};
		return model;
	}

	/// <summary>
	/// https://platform.openai.com/docs/api-reference/models
	/// </summary>
	internal async Task<OpenAIModelResponse?> GetModelsAsync()
	{
		using var httpClient = new HttpClient();
		if (!string.IsNullOrWhiteSpace(_config.ApiKey))
		{
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);
		}

		try
		{
			string json = await httpClient.GetStringAsync($"{_config.ApiHostUrl}/v1/models");
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new Exception("No data returned from API");
			}
			return JsonSerializer.Deserialize<OpenAIModelResponse>(json, _serializerOptions); ;

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"An error occurred: {ex.Message}");
			return null;
		}
	}

	private List<string> SplitTextInParts(string input, int maxWords)
	{
		var words = input.Split(new[] { ' ' });
		var parts = new List<string>();
		for (int i = 0; i < words.Length; i += maxWords)
		{
			var part = string.Join(" ", words.Skip(i).Take(maxWords));
			parts.Add(part);
		}
		return parts;
	}

	private JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
	{
		WriteIndented = true,
		PropertyNameCaseInsensitive = true,
		AllowTrailingCommas = true,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
		ReadCommentHandling = JsonCommentHandling.Skip,
	};
}

/*
{
  "object": "list",
  "data": [
    {
      "id": "model-id-0",
      "object": "model",
      "created": 1686935002,
      "owned_by": "organization-owner"
    },
    {
	"id": "model-id-1",
      "object": "model",
      "created": 1686935002,
      "owned_by": "organization-owner",
    },
    {
	"id": "model-id-2",
      "object": "model",
      "created": 1686935002,
      "owned_by": "openai"

	},
  ],
  "object": "list"
}
*/

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class OpenAIModelResponse
{
	public string? @Object { get; set; }
	public List<Model>? Data { get; set; }

	public class Model
	{
		public string? Id { get; set; }
		public string? @Object { get; set; }
		public int? Created { get; set; }
		[JsonPropertyName("owned_by")]
		public string? OwnedBy { get; set; }
	}
}

/*
{ 
  "messages": [
    { "role": "system", "content": "You are a helpful coding assistant." },
    { "role": "user", "content": "How do I init and update a git submodule?" }
  ], 
  "temperature": 0.7, 
  "max_tokens": -1,
  "stream": true
}
*/

// Additional options in LM Studio supported: https://platform.openai.com/docs/api-reference/chat/create
public class OpenAICompletionsRequest
{
    public string Model { get; set; }
    public List<OpenAIMessage> Messages { get; set; } = new();
	public float Temperature { get; set; } = 0.7f;

	[JsonPropertyName("max_tokens")]
	public int MaxTokens { get; set; } = -1;
	public bool Stream { get; set; } = false;
	[JsonPropertyName("response_format")]
	public object ResponseFormat { get; set; } = new { type = "json_object" };
}

public class OpenAIMessage
{
	public OpenAICompletionRole Role { get; set; } = OpenAICompletionRole.User;
	public string Content { get; set; } = "";
}

public enum OpenAICompletionRole
{
	[JsonPropertyName("assistant")]
	Assistant,
	[JsonPropertyName("system")]
	System,
	[JsonPropertyName("user")]
	User
}


public class OpenAICompletionsResponse
{
/*
{
  "id": "chatcmpl-123",
  "object": "chat.completion",
  "created": 1677652288,
  "model": "gpt-3.5-turbo-0125",
  "system_fingerprint": "fp_44709d6fcb",
  "choices": [{
    "index": 0,
    "message": {
      "role": "assistant",
      "content": "\n\nHello there, how may I assist you today?",
    },
    "logprobs": null,
    "finish_reason": "stop"
  }],
  "usage": {
	"prompt_tokens": 9,
    "completion_tokens": 12,
    "total_tokens": 21
  }
}
*/
    public string Id { get; set; }
	public string Object { get; set; }
	public int Created { get; set; }		
	public string Model { get; set; }
	[JsonPropertyName("system_fingerprint")]
	public string? SystemFingerprint { get; set; }
	public List<OpenAIChoice> Choices { get; set; }
	public OpenAIUsage Usage { get; set; }
}



public class OpenAIChoice
{
	public int Index { get; set; }
	public OpenAIMessage Message { get; set; }
	public object Logprobs { get; set; }
	[JsonPropertyName("finish_reason")]
	public string? FinishReason { get; set; }
}

public class OpenAIUsage
{
	[JsonPropertyName("prompt_tokens")]
	public int PromptTokens { get; set; }
	[JsonPropertyName("completion_tokens")]
	public int CompletionTokens { get; set; }
	[JsonPropertyName("total_tokens")]
	public int TotalTokens { get; set; }
}
