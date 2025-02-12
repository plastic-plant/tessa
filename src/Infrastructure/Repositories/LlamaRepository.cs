﻿using LLama;
using LLama.Abstractions;
using LLama.Common;
using Microsoft.Extensions.Logging;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Application.Models.ProviderConfigs;

namespace Tessa.Infrastructure.Repositories;

public class LlamaRepository: ILlamaRepository
{
	private readonly ILogger<LlamaRepository> _logger;
	private readonly IServiceProvider _services;
	private readonly ISettingsService _settings;
	private ProviderConfigLlamaGguf? _config;
	private LLamaWeights? _model;
	private ILLamaExecutor? _executor;

	public LlamaRepository(ILogger<LlamaRepository> logger, IServiceProvider services, ISettingsService settings)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
	}

	public (bool ready, string? error) IsReady()
	{
		_config = _config ?? _settings.Settings?.GetSelectedProviderConfiguration() as ProviderConfigLlamaGguf;
		try
		{
			var executor = GetModelProvider();
			if (executor == null)
			{
				throw new Exception("Executor returns null.");
			}
		}
		catch (Exception ex)
		{
			return (false, $"Could not open LLM: {ex.Message}");
		}
		
		return (true, null);
	}

	public async Task<FileSummary> ProcessAsync(FileSummary file)
	{
		_config = _config ?? _settings.Settings.GetSelectedProviderConfiguration() as ProviderConfigLlamaGguf;
		var executor = GetModelProvider();
		var parameters = new InferenceParams()
		{
			Temperature = _config.Temperature,
			MaxTokens = _config.MaxTokens
		};

		var content = File.ReadAllText(file.FilePathResultOcr);
		var parts = SplitTextInParts(content, _config.MaxPrompt);
		using var response = new StringWriter();
		foreach (var part in parts)
		{
			var prompt = _config.CleanupPrompt!.Replace("<CONTENT>", part);
			await foreach (var text in executor.InferAsync(prompt, parameters))
			{
				response.Write(text);
			}
		}

		file.FilePathResultLlm = Path.Combine(_settings.Settings.Ocr.OutputPath, $"{file.FileNameWithoutExtension!}.prompt.txt");
		File.WriteAllText(file.FilePathResultOcr, response.ToString());

		return file;
	}

	private ILLamaExecutor GetModelProvider()
	{
		var modelParameters = new ModelParams(_config.Model)
		{
			ContextSize = _config.ContextSize,
			Seed = _config.Seed,
			GpuLayerCount = _config.GpuLayerCount
		};

		_model = _model ?? LLamaWeights.LoadFromFile(modelParameters);
		_executor = _executor ?? new StatelessExecutor(_model, modelParameters);

		return _executor;
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

	public void Dispose()
	{
		_model?.Dispose();
	}
}
