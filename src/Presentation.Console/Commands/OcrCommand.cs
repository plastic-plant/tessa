﻿using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Infrastructure.Extensions;
using Tessa.Presentation.Cli.Enums;

namespace Tessa.Presentation.Cli.Commands;

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
		var appsettings = _settingsService
			.LoadAppSettingsFromFile(settings.SettingsPath)
			.Override(loaded => loaded.Ocr.InputPath, settings.InputPath, AppSettings.OcrSettings.Defaults.InputPath)
			.Override(loaded => loaded.Ocr.OutputPath, settings.OutputPath, AppSettings.OcrSettings.Defaults.OutputPath)
			.Override(loaded => loaded.Ocr.LanguageTessdata, settings.TessdataLanguage, AppSettings.OcrSettings.Defaults.TessdataLanguage);
		
		string[] errors = [.. appsettings.Errors, .. _ocrService.Validate().Errors];
		if (errors.Count() > 0)
		{
			return ValidationResult.Error(string.Join(" ", errors));
		}

		return base.Validate(context, settings);
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
	{
		await _ocrService.Execute();
		return (int)ExitCode.OK;
	}
}