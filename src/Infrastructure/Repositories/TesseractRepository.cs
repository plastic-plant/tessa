using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
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
	private TesseractEngine? _engine;

	public TesseractRepository(ILogger<TesseractRepository> logger, IServiceProvider services, ISettingsService settings)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings?.Settings?.Ocr ?? throw new ArgumentNullException(nameof(settings));
	}

	public (string? version, string? error) IsReady()
	{
		try
		{
			_engine = _engine = new TesseractEngine(@"./tessdata", _settings.TessdataLanguage);
			return (_engine?.Version, null);
		}
		catch (Exception ex)
		{
			return (null, ex.StackTrace);
		}
	}

	public FileSummary Process(FileSummary file)
	{
		_engine = _engine ?? new TesseractEngine(@"./tessdata", _settings.TessdataLanguage);
		if (file.IsImage)
		{
			using var image = Pix.LoadFromFile(file.FilePathRooted);
			using var page = _engine.Process(image);
			var text = page.GetText();
			file.FilePathResultOcr = Path.Combine(_settings.OutputPath, $"{file.FileNameWithoutExtension!}.orc.txt");
			file.Confidence = page.GetMeanConfidence();
			File.WriteAllText(file.FilePathResultOcr, text);
			_logger.LogDebug($"Tesseract processed {file.FileName} with confidence {file.Confidence}");
			// TODO: status and error handling, skip-message
		}
		return file;
	}

	public void Dispose()
	{
		_engine?.Dispose();
	}
}
