using Microsoft.Extensions.DependencyInjection;
using Tessa.Application.Enums;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;

namespace Tessa.Application.Services;

public class OcrService: IOcrService
{
	private readonly IServiceProvider _services;
	private readonly AppSettings.OcrSettings _settings;
	private readonly IFileRepository _files;

	private ITesseractRepository? _tesseract;

	public OcrService(IServiceProvider services, ISettingsService settings, IFileRepository files)
    {
		_services = services ?? throw new ArgumentNullException(nameof(services));
		_settings = settings?.Settings?.Ocr ?? throw new ArgumentNullException(nameof(settings));
		_files = files ?? throw new ArgumentNullException(nameof(files));
	}

	public OcrSummary Validate()
	{
		var summary = new OcrSummary();

		var path = _files.GetPathSummary(_settings.InputPath);
		if (!path.IsExistingDirectory && !path.IsExistingFile)
		{
			summary.Errors.Add($"Could not open folder or file {path.PathRooted}.");
		}

		switch (_settings.Engine)
		{
			case OcrEngine.Tesseract:
				_tesseract = _tesseract ?? _services.GetService<ITesseractRepository>();
				(_, string? error) = _tesseract?.IsReady() ?? (null, "Implementation for ITesseractRepository not found.");
				if (error != null)
				{
					summary.Errors.Add($"Could not start Tesseract engine: {error}");
				}
				break;
		}

		return summary;
	}

	public async Task<OcrSummary> Execute()
	{
		await Task.Delay(1000);
		var files = _files.GetFilesSummary(_settings.InputPath);

		switch (_settings.Engine)
		{
			case OcrEngine.Tesseract:
				_tesseract = _tesseract ?? _services.GetRequiredService<ITesseractRepository>();
				foreach (var file in files)
				{
					_tesseract.Process(file);
				}
				break;

			default:
				break;
		}
		return new();
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
		_tesseract?.Dispose();
	}
}
