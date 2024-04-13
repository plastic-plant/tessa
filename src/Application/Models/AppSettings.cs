using System.Text.Json.Serialization;
using Tessa.Application.Enums;

namespace Tessa.Application.Models;

public class AppSettings
{
	public class Defaults
	{
		public const string SettingsPath = "tessa.settings.json";
		public const string LogPath = "logs";
	}
	
	[JsonIgnore]
	public List<string> Errors { get; set; } = new();
	
	[JsonIgnore]
	public string SettingsPath { get; set; } = Defaults.SettingsPath;

	[JsonPropertyName("log")]
	public string LogPath { get; set; } = Defaults.LogPath;

	public OcrSettings Ocr { get; set; } = new();

    public class OcrSettings
	{
		public class Defaults
		{
			public const string InputPath = "input";
			public const string OutputPath = "output";
			public const OcrEngine Engine = OcrEngine.Tesseract;
			public const string TessdataPath = "tessdata";
			public const string TessdataLanguage = "eng";
			public const string LlmPromptConfigName = "geitje-7b-ultra";
		}

		[JsonPropertyName("in")]
		public string InputPath { get; set; } = Defaults.InputPath;

		[JsonPropertyName("out")]
		public string OutputPath { get; set; } = Defaults.OutputPath;		

		[JsonPropertyName("engine")]
		public OcrEngine Engine { get; set; } = Defaults.Engine;

		[JsonPropertyName("tessdata")]
		public string TessdataPath { get; set; } = Defaults.TessdataPath;

		[JsonPropertyName("lang")]
		public string TessdataLanguage { get; set; } = Defaults.TessdataLanguage;

		[JsonPropertyName("llm")]
		public string LlmPromptConfigName { get; set; } = Defaults.LlmPromptConfigName;
	}

    public LlmSettings Llm { get; set; } = new();
    public class LlmSettings
	{
        public class Defaults
		{
			public static List<LlmConfigExample> GetConfigExamples() => new()
			{
				new LlmConfigExample { Name = "OpenAI", Provider = LlmProvider.OpenAI, Model = "gpt-3.5-turbo", ContextSize = 16385, Seed = 42, GpuLayerCount = 5, Temperature = 0.7f, MaxTokens = -1, MaxPrompt = 200, Prompt = "Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text. Here's the text:", SuggestedPrompts = ["Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text.  Here's the text:"] },
				new LlmConfigExample { Name = "wizardLM-7B", Provider = LlmProvider.Llama, Model = "C:\\Users\\Patrick\\.cache\\lm-studio\\models\\TheBloke\\wizardLM-7B-GGUF\\wizardLM-7B.Q8_0.gguf", ContextSize = 2048, Seed = 42, GpuLayerCount = 5, Temperature = 0.7f, MaxTokens = -1, MaxPrompt = 200, Prompt = "Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text. Here's the text:", SuggestedPrompts = ["Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text.  Here's the text:"] },
				new LlmConfigExample { Name = "llama-2-7b-chat", Provider = LlmProvider.Llama, Model = "C:\\Users\\Patrick\\.cache\\lm-studio\\models\\TheBloke\\Llama-2-7B-Chat-GGUF\\llama-2-7b-chat.Q3_K_M.gguf", ContextSize = 4096, Seed = 42, GpuLayerCount = 5, Temperature = 0.7f, MaxTokens = -1, MaxPrompt = 200, Prompt = "Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text. Here's the text:", SuggestedPrompts = ["Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text.  Here's the text:"] },
				new LlmConfigExample { Name = "gemma-2b-it", Provider = LlmProvider.Llama, Model = "C:\\Users\\Patrick\\.cache\\lm-studio\\models\\lmstudio-ai\\gemma-2b-it-GGUF\\gemma-2b-it.gguf", ContextSize = 1024, Seed = 42, GpuLayerCount = 5, Temperature = 0.6f, MaxTokens = -1, MaxPrompt = 200, Prompt = "Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text.  Here's the text:", SuggestedPrompts = ["Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text.  Here's the text:"] },
				new LlmConfigExample { Name = "geitje-7b-ultra", Provider = LlmProvider.Llama, Model = "C:\\Users\\Patrick\\.cache\\lm-studio\\models\\EgoisticFoil\\GEITje-7B-ultra-GGUF\\geitje-7b-ultra.Q8_0.gguf", ContextSize = 32768, Seed = 42, GpuLayerCount = 5, Temperature = 0.6f, MaxTokens = -1, MaxPrompt = 200, Prompt = "Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text.  Here's the text:", SuggestedPrompts = ["Clean up text. Avoid confirmation messages in your response. If you can't provide an answer, leave the response empty. Answer in the language of given text.  Here's the text:"] },
			};
		}

		public class LlmConfig
		{
			public required string Name { get; set; }
			public LlmProvider Provider { get; set; }
            public string? ApiKey { get; set; }
            public string? Model { get; set; }			
			public uint? ContextSize { get; set; }
			public uint Seed { get; set; }
			public int GpuLayerCount { get; set; }
			public float Temperature { get; set; }
            public int MaxTokens { get; set; }
			public int MaxPrompt { get; set; }
			public string? Prompt { get; set; }
		}

		public class LlmConfigExample : LlmConfig
		{
			[JsonPropertyName("suggestions")]
			public List<string>? SuggestedPrompts { get; set; }
		}

		[JsonPropertyName("configs")]
		public List<LlmConfigExample> LlmConfigs { get; set; } = Defaults.GetConfigExamples();

	}
}
