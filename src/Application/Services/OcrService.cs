using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tessa.Application.Enums;
using Tessa.Application.Events;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Application.Models.ProviderConfigs;

namespace Tessa.Application.Services;

public class OcrService: IOcrService
{
	private readonly ILogger<OcrService> _logger;
	private readonly IServiceProvider _services;
	private readonly ISettingsService _settings;
	private readonly IFileRepository _files;

	private ITesseractRepository? _tesseract;
	private ILlamaRepository? _llama;
	private IOpenAIRepository? _openai;

	public OcrService(ILogger<OcrService> logger, IServiceProvider services, ISettingsService settings, IFileRepository files)
    {
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		_files = files ?? throw new ArgumentNullException(nameof(files));
	}

	public event ProgressEventHandler? ProgressChanged;

	public OcrSummary Validate()
	{
		var summary = new OcrSummary();

		// Validate avilability of paths.
		var path = _files.GetPathSummary(_settings.Settings.Ocr.InputPath);
		if (!path.IsExistingDirectory && !path.IsExistingFile)
		{
			summary.Errors.Add($"Could not open folder or file {path.PathRooted}.");
		}

		// Validate availability of OCR engine.
		switch (_settings.Settings.Ocr.Engine)
		{
			case OcrEngine.Tesseract:
				_tesseract = _tesseract ?? _services.GetService<ITesseractRepository>();
				var status = _tesseract!.IsReady();
				if (!status.ready)
				{
					summary.Errors.Add($"Could not start Tesseract engine: {status.error}");
				}
				break;
		}

		// Validate availability of provider for LLM prompting.
		var config = _settings.Settings.GetSelectedProviderConfiguration();
		switch (config?.Provider)
		{
			case LlmProvider.Llama:
				_llama = _llama ?? _services.GetRequiredService<ILlamaRepository>();
				(bool ready, string? error) = _llama.IsReady();
				if (error != null)
				{
					summary.Errors.Add($"Could not start LLaMa engine: {error}");
				}
				break;

			case LlmProvider.OpenAI:
			case LlmProvider.LMStudio:
			case LlmProvider.Jan:
				_openai = _openai ?? _services.GetService<IOpenAIRepository>();
				var result = _openai?.IsReadyAsync().Result;
				if (!result.HasValue || !result.Value.ready)
				{
					summary.Errors.Add($"Could not read provider API: {result!.Value.error}");
				}
				break;

			default:
				summary.Errors.Add($"Could not find provider configuration for prompting: {_settings.Settings.Ocr.SelectedProviderConfigName}.");
				break;
		}

		// Validate availability of cleanup prompt.
		if (string.IsNullOrWhiteSpace(_settings.Settings.Ocr.CleanupPrompt))
		{
			summary.Errors.Add("Prompt for OCR optimalization not configured. Please use one of the examples in tessa.settings.json.");
		}

		return summary;
	}

	public async Task<OcrSummary> Execute()
	{
		var summary = new OcrSummary();
		summary.Files = _files.GetFilesSummary(_settings.Settings.Ocr.InputPath);

		int position = 0;
		foreach (var file in summary.Files)
		{			
			summary.CurrentFile = file;
			summary.CurrentFilePosition = ++position;

			// Process file with OCR engine.
			switch (_settings.Settings.Ocr.Engine)
			{
				case OcrEngine.Tesseract:
					_tesseract = _tesseract ?? _services.GetRequiredService<ITesseractRepository>();
					_logger.LogInformation($"Processing file {position} with Tesseract.", file.FileName);
					file.OcrProcessingStatus = OcrProcessingStatus.Processing;
					ProgressChanged?.Invoke(this, new ProgressEventArgs(summary));
					_tesseract.Process(file);
					break;
				default:
					break;
			}

			// Process file with LLM prompting.
			var llmConfig = _settings.Settings.GetSelectedProviderConfiguration() as ProviderConfig;
			_logger.LogInformation($"Optimizing file {file.FileName} with prompting.");
			file.OcrProcessingStatus = OcrProcessingStatus.Optimizing;
			ProgressChanged?.Invoke(this, new ProgressEventArgs(summary));
			switch (llmConfig?.Provider)
			{
				case LlmProvider.Llama:
					_llama = _llama ?? _services.GetRequiredService<ILlamaRepository>();
					await _llama.ProcessAsync(file);
					_llama?.Dispose();
					break;

				case LlmProvider.OpenAI:
				case LlmProvider.LMStudio:
				case LlmProvider.Jan:
					_openai = _openai ?? _services.GetRequiredService<IOpenAIRepository>();
					await _openai.ProcessAsync(file);
					break;

				default:
					break;
			}

			// Finish processing file.
			file.OcrProcessingStatus = OcrProcessingStatus.Finished;
			ProgressChanged?.Invoke(this, new ProgressEventArgs(summary));
			_logger.LogInformation($"Finished processing file.");
		}

		return summary;
	}

	public async Task<OcrSummary> Cancel()
	{
		await Task.Delay(1000);
		throw new NotImplementedException();
	}

	public async Task<OcrSummary> Pause()
	{
		await Task.Delay(1000);
		throw new NotImplementedException();
	}

	public async Task<OcrSummary> Stop()
	{
		await Task.Delay(1000);
		throw new NotImplementedException();
	}

	public async Task<OcrSummary> Verify()
	{
		await Task.Delay(1000);
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		_llama?.Dispose();
	}
}
