using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using Tessa.Application.Enums;
using Tessa.Application.Events;
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

	public event ProgressEventHandler? ProgressChanged;

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
		var summary = new OcrSummary();
		summary.Files = _files.GetFilesSummary(_settings.InputPath);

		switch (_settings.Engine)
		{
			case OcrEngine.Tesseract:
				_tesseract = _tesseract ?? _services.GetRequiredService<ITesseractRepository>();
				
				int position = 0;
				foreach (var file in summary.Files)
				{
					summary.CurrentFile = file;
					summary.CurrentFilePosition = ++position;
					
					file.OcrProcessingStatus = OcrProcessingStatus.Processing;
					ProgressChanged?.Invoke(this, new ProgressEventArgs(summary));

					file.OcrProcessingStatus = OcrProcessingStatus.Optimizing;
					await Task.Delay(2000);
					ProgressChanged?.Invoke(this, new ProgressEventArgs(summary));

					file.OcrProcessingStatus = OcrProcessingStatus.Finished;
					_tesseract.Process(file);
					ProgressChanged?.Invoke(this, new ProgressEventArgs(summary));
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
