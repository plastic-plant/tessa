using Microsoft.Extensions.Logging;
using Tessa.Application.Enums;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tesseract;

namespace Tessa.Infrastructure.Tesseract;

public class TesseractRepository : ITesseractRepository
{
	private readonly ILogger<TesseractRepository> _logger;
	private readonly IServiceProvider _services;
	private readonly AppSettings.OcrSettings _settings;

	public TesseractRepository(ILogger<TesseractRepository> logger, IServiceProvider services, ISettingsService settings)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings?.Settings?.Ocr ?? throw new ArgumentNullException(nameof(settings));
	}

	public (bool ready, string? error) IsReady()
	{
		try
		{
			// Verify that the requested Tesseract models are available.
			foreach (var language in _settings.TessdataLanguage.Split("+"))
			{
				var tesseractmodel = Path.Combine(_settings.TessdataPath, $"{language}.traineddata");
				if (!File.Exists(tesseractmodel))
				{
					return (false, $"Could not find Tesseract model for language {language} at path: {tesseractmodel}. Type: tessa download tessdata {language}");
				}
			}

			// Verify that Tesseract can be called with the given parameters.
			using var engine = new TesseractEngine(_settings.TessdataPath, _settings.TessdataLanguage);
			return (!string.IsNullOrWhiteSpace(engine.Version), null);
		}
		catch (Exception ex)
		{
			return (false, ex.Message);
		}
	}

	public FileSummary Process(FileSummary file)
	{
		try
		{
			using var engine = new TesseractEngine(_settings.TessdataPath, _settings.TessdataLanguage, EngineMode.LstmOnly);

			// Process image file.
			if (file.IsImage)
			{
				using var image = Pix.LoadFromFile(file.FilePathRooted);
				using var page = engine.Process(image);
				var text = page.GetText();
				file.FilePathResultOcr = Path.Combine(_settings.OutputPath, $"{file.FileNameWithoutExtension!}.ocr.txt");
				file.Confidence = page.GetMeanConfidence();
				File.WriteAllText(file.FilePathResultOcr, text);
				_logger.LogDebug($"Tesseract processed {file.FileName} with confidence {file.Confidence}");
			}
		}
		catch (Exception ex)
		{
			file.OcrProcessingStatus = OcrProcessingStatus.Failed;
			var failMessage = $"Tesseract failed to process {file.FileName}: {ex.Message}";
			_logger.LogError(ex, failMessage);
			file.Errors.Add(failMessage);
		}
		return file;
	}
}
