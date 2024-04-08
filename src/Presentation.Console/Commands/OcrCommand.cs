using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Presentation.Console.Helpers;
using Tessa.Presentation.Console.Enums;
using System;
using System.Diagnostics;
using Tessa.Application.Events;

namespace Tessa.Presentation.Console.Commands;

public sealed class OcrCommand : AsyncCommand<OcrCommand.Settings>
{
	private readonly ISettingsService _settingsService;
	private readonly IOcrService _ocrService;

	public sealed class Settings : CommandSettings
	{
		[CommandOption("--settings <filename>")]
		[Description($"Use an alternative settings file")]
		[DefaultValue(AppSettings.Defaults.SettingsPath)]
		public required string SettingsPath { get; set; }

		[CommandOption("--in <folder_or_filename>")]
		[Description($"Use an alternative input folder or file to OCR scan")]
		[DefaultValue(AppSettings.OcrSettings.Defaults.InputPath)]
		public required string InputPath { get; set; }

		[CommandOption("--out <folder_or_filename>")]
		[Description($"Use an alternative output folder or file for OCR transcriptions, add a file extension [grey].txt, .html, .pdf, .docx[/] to format output.")]
		[DefaultValue(AppSettings.OcrSettings.Defaults.OutputPath)]
		public required string OutputPath { get; set; }

		[CommandOption("--lang <tessdata_language>")]
		[Description($"Use an alternative language model with Tesseract")]
		[DefaultValue(AppSettings.OcrSettings.Defaults.TessdataLanguage)]
		public required string TessdataLanguage { get; set; }
	}

	public OcrCommand(ISettingsService settingsService, IOcrService ocrService)
	{
		_settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
		_ocrService = ocrService ?? throw new ArgumentNullException(nameof(ocrService));
	}

	public override ValidationResult Validate(CommandContext context, Settings settings)
	{
		var appsettings = _settingsService.Load(settings.SettingsPath);

		CommandHelpers.ApplySetting(appsettings.Ocr.InputPath, settings.InputPath, AppSettings.OcrSettings.Defaults.InputPath);
		CommandHelpers.ApplySetting(appsettings.Ocr.OutputPath, settings.OutputPath, AppSettings.OcrSettings.Defaults.OutputPath);
		CommandHelpers.ApplySetting(appsettings.Ocr.LanguageTessdata, settings.TessdataLanguage, AppSettings.OcrSettings.Defaults.TessdataLanguage);

		string[] errors = [.. appsettings.Errors, .. _ocrService.Validate().Errors];
		if (errors.Count() > 0)
		{
			return ValidationResult.Error(string.Join(" ", errors));
		}

		return base.Validate(context, settings);
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
	{
		_ocrService.ProgressChanged += ProgressChanged;
		await _ocrService.Execute();
		_ocrService.ProgressChanged -= ProgressChanged;
		return (int)ExitCode.OK;
	}

	void ProgressChanged(object sender, ProgressEventArgs e)
	{
		System.Console.WriteLine($"Progress: {e.ProgressPercentage}% File {e.Summary.CurrentFilePosition}: {e.Summary.CurrentFile?.FileName}");
	}
}