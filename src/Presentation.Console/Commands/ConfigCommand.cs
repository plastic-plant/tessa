﻿using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Tessa.Application.Enums;
using Tessa.Application.Interface;
using Tessa.Application.Models;
using Tessa.Application.Services;
using Tessa.Presentation.Console.Enums;

namespace Tessa.Presentation.Console.Commands;

public class ConfigCommand : Command<ConfigCommand.Settings>
{
	private readonly ILogger<OcrCommand> _logger;
	private readonly ISettingsService _settingsService;

	public class Settings: CommandSettings
	{
		[CommandOption("--settings <filename>")]
		[Description("Use an alternative settings file")]
		[DefaultValue(AppSettings.Defaults.SettingsPath)]
		public required string SettingsPath { get; set; }
	}

	public ConfigCommand(ILogger<OcrCommand> logger, ISettingsService settingsService)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
	}

	public override ValidationResult Validate(CommandContext context, Settings settings)
	{
		_settingsService.LoadSettings(settings.SettingsPath);
		_settingsService.LoadRegistry();
		return base.Validate(context, settings);
	}

	public override int Execute(CommandContext context, Settings settings)
	{
		var gotoMenu = ConfigMenu.MainMenu;
		while (gotoMenu != ConfigMenu.Quit)
		{
			gotoMenu = gotoMenu switch
			{
				ConfigMenu.MainMenu => ShowMainMenu(),
				ConfigMenu.ChangeLogPath => ShowMenu1_ChangeLogPath(),
				ConfigMenu.ChangeInputPath => ShowMenu2_ChangeInputPath(),
				ConfigMenu.ChangeOutputLanguage => ShowMenu3_ChangeOutputLanguage(),
				ConfigMenu.ChangeTessdataPath => ShowMenu4_ChangeOcrModelsPath(),
				ConfigMenu.ChangeModelsPath => ShowMenu5_ChangeLlmModelsPath(),
				ConfigMenu.ChangeOcrEngine => ShowMenu6_ChangeOcrEngine(),
				ConfigMenu.ChangeOcrModel => ShowMenu7_ChangeOcrModel(),
				ConfigMenu.ChangeLlmProvider => ShowMenu8_ChangeLlmProvider(),
				ConfigMenu.ChangeOcrPrompt => ShowMenu9_ChangeOcrPrompt(),
				_ => ShowMainMenu()
			};
		}

		return (int)ExitCode.OK;
	}

	private ConfigMenu ShowMainMenu()
	{
		AnsiConsole.Clear();
		var answer = AnsiConsole
			.Prompt(new SelectionPrompt<string>()
			.Title("\nWelcome to [underline green]tessa config[/]. I'm here to help you update settings in [purple]tessa.settings.json[/].\n")
			.PageSize(10)
			.AddChoices(
				"1. Change the path where [lightskyblue1]log files[/] are written.",
				"2. Change the path where [lightskyblue1]images and documents[/] are read.",
				"3. Change the path where [lightskyblue1]text files[/] after OCR are written.",
				"4. Change the path where [lightskyblue1]Tesseract tessdata models[/] are stored.",
				"5. Change the path where [lightskyblue1]Large language models[/] are stored.",
				$"6. Change the [lightskyblue1]OCR engine[/] selected: [green]{_settingsService.Settings.Ocr.Engine}[/]",
				$"7. Change the [lightskyblue1]OCR language[/] models: [green]{_settingsService.Settings.Ocr.TessdataLanguage}[/]",
				$"8. Change the [lightskyblue1]LLM prompting[/] model: [green]{_settingsService.Settings.Ocr.SelectedProviderConfigName}[/]",
				"9. Change the cleanup strategy and [lightskyblue1]prompt to optimize[/] text readouts.",
				"Q. Quit configuration"
			));

		return answer[0] switch
		{
			'1' => ConfigMenu.ChangeLogPath,
			'2' => ConfigMenu.ChangeInputPath,
			'3' => ConfigMenu.ChangeOutputLanguage,
			'4' => ConfigMenu.ChangeTessdataPath,
			'5' => ConfigMenu.ChangeModelsPath,
			'6' => ConfigMenu.ChangeOcrEngine,
			'7' => ConfigMenu.ChangeOcrModel,
			'8' => ConfigMenu.ChangeLlmProvider,
			'9' => ConfigMenu.ChangeOcrPrompt,
			'Q' => ConfigMenu.Quit,
			_ => ConfigMenu.MainMenu
		};
	}

	private ConfigMenu ShowMenu1_ChangeLogPath()
	{
		AnsiConsole.Clear();
		AnsiConsole.MarkupLine($"\nPress Enter to save the current configured default.\n\n");
		var textPath = new TextPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settingsService.Settings.LogPath))
						.RootColor(Color.Red)
						.SeparatorColor(Color.Green)
						.StemColor(Color.Blue)
						.LeafColor(Color.Yellow);
		var panel = new Panel(textPath);
					panel.Header = new PanelHeader("1. Change the path where [lightskyblue1]log files[/] are written");
					panel.Border = BoxBorder.Rounded;
					panel.Expand = true;
					AnsiConsole.Write(panel);

					var answer = AnsiConsole.Ask<string>("\n\n", _settingsService.Settings.LogPath);
		_settingsService.Settings.LogPath = answer;
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
	}

	private ConfigMenu ShowMenu2_ChangeInputPath()
	{
		AnsiConsole.Clear();
		AnsiConsole.MarkupLine($"\nPress Enter to save the current configured default.\n\n");
		var textPath = new TextPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settingsService.Settings.Ocr.InputPath))
			.RootColor(Color.Red)
			.SeparatorColor(Color.Green)
			.StemColor(Color.Blue)
			.LeafColor(Color.Yellow);
		var panel = new Panel(textPath);
		panel.Header = new PanelHeader("2. Change the path where [lightskyblue1]images and documents[/] are read");
		panel.Border = BoxBorder.Rounded;
		panel.Expand = true;
		AnsiConsole.Write(panel);

		var answer = AnsiConsole.Ask<string>("\n\n", _settingsService.Settings.Ocr.InputPath);
		_settingsService.Settings.Ocr.InputPath = answer;
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
	}

	private ConfigMenu ShowMenu3_ChangeOutputLanguage()
	{
		AnsiConsole.Clear();
		AnsiConsole.MarkupLine($"\nPress Enter to save the current configured default.\n\n");
		var textPath = new TextPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settingsService.Settings.Ocr.OutputPath))
			.RootColor(Color.Red)
			.SeparatorColor(Color.Green)
			.StemColor(Color.Blue)
			.LeafColor(Color.Yellow);
		var panel = new Panel(textPath);
		panel.Header = new PanelHeader("3. Change the path where [lightskyblue1]text files[/] after OCR are written");
		panel.Border = BoxBorder.Rounded;
		panel.Expand = true;
		AnsiConsole.Write(panel);

		var answer = AnsiConsole.Ask<string>("\n\n", _settingsService.Settings.Ocr.OutputPath);
		_settingsService.Settings.Ocr.OutputPath = answer;
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
	}

	private ConfigMenu ShowMenu4_ChangeOcrModelsPath()
					{
		AnsiConsole.Clear();
		AnsiConsole.MarkupLine($"\nPress Enter to save the current configured default.\n\n");
		var textPath = new TextPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settingsService.Settings.Ocr.TessdataPath))
			.RootColor(Color.Red)
			.SeparatorColor(Color.Green)
			.StemColor(Color.Blue)
			.LeafColor(Color.Yellow);
		var panel = new Panel(textPath);
		panel.Header = new PanelHeader("4. Change the path where [lightskyblue1]Tesseract tessdata models[/] are stored");
		panel.Border = BoxBorder.Rounded;
		panel.Expand = true;
		AnsiConsole.Write(panel);

		var answer = AnsiConsole.Ask<string>("\n\n", _settingsService.Settings.Ocr.TessdataPath);
		_settingsService.Settings.Ocr.TessdataPath = answer;
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
					}

	private ConfigMenu ShowMenu5_ChangeLlmModelsPath()
	{
		AnsiConsole.Clear();
		AnsiConsole.MarkupLine($"\nPress Enter to save the current configured default.\n\n");
		var textPath = new TextPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settingsService.Settings.Llm.ModelsPath))
			.RootColor(Color.Red)
			.SeparatorColor(Color.Green)
			.StemColor(Color.Blue)
			.LeafColor(Color.Yellow);
		var panel = new Panel(textPath);
		panel.Header = new PanelHeader("5. Change the path where [lightskyblue1]Large language models[/] are stored");
		panel.Border = BoxBorder.Rounded;
		panel.Expand = true;
		AnsiConsole.Write(panel);

		var answer = AnsiConsole.Ask<string>("\n\n", _settingsService.Settings.Llm.ModelsPath);
		_settingsService.Settings.Llm.ModelsPath = answer;
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
			}

	private ConfigMenu ShowMenu6_ChangeOcrEngine()
	{
		AnsiConsole.Clear();
		var choices = Enum.GetValues(typeof(OcrEngine)).Cast<OcrEngine>();
		var answer = AnsiConsole
			.Prompt(new SelectionPrompt<OcrEngine>()
				.Title($"6. Change the [lightskyblue1]OCR engine[/] selected: [green]{_settingsService.Settings.Ocr.Engine}[/]\n")
				.PageSize(10)
				.AddChoices(choices)
			);

		_settingsService.Settings.Ocr.Engine = answer;
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
		}

	private ConfigMenu ShowMenu7_ChangeOcrModel()
	{
		AnsiConsole.Clear();
		AnsiConsole.MarkupLine($"\nPress Enter to save the current configured default.\n\n");
		var choices = _settingsService.Registry.Tessdata.Select(model => model.Alias);
		var answers = AnsiConsole
			.Prompt(new  MultiSelectionPrompt<string>()
				.Title($"7. Change the [lightskyblue1]OCR language[/] models: [green]{_settingsService.Settings.Ocr.TessdataLanguage}[/]\n")
				.MoreChoicesText("[grey](Move up and down to scroll through more language models.)[/]")
				.PageSize(20)
				.AddChoices(choices)
			);

		_settingsService.Settings.Ocr.TessdataLanguage = string.Join('+', answers);
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
	}

	private ConfigMenu ShowMenu8_ChangeLlmProvider()
	{
		AnsiConsole.Clear();
		AnsiConsole.MarkupLine($"\nPress Enter to save the current configured default.\n\n");
		var choices = _settingsService.Settings.Llm.ProviderConfigurations.Select(model => model.Name ?? "");
		var answer = AnsiConsole
			.Prompt(new SelectionPrompt<string>()
				.Title($"8. Change the [lightskyblue1]LLM prompting[/] model: [green]{_settingsService.Settings.Ocr.SelectedProviderConfigName}[/]\n")
				.MoreChoicesText("[grey](Move up and down to scroll through more language models.)[/]")
				.PageSize(10)
				.AddChoices(choices)
			);

		_settingsService.Settings.Ocr.SelectedProviderConfigName = answer;
		_settingsService.SaveSettings();

		return ConfigMenu.MainMenu;
	}

	private ConfigMenu ShowMenu9_ChangeOcrPrompt()
	{
		AnsiConsole.Clear();

		var config = _settingsService.Settings.GetSelectedProviderConfiguration();
		var answer = AnsiConsole.Ask<string>("9. Change the cleanup strategy and [lightskyblue1]prompt to optimize[/] text readouts.\n\n[grey]Press Enter to save the current configured default: [/]", AppSettings.OcrSettings.Defaults.CleanupPrompt);
		if (!string.IsNullOrWhiteSpace(answer))
		{
			config.CleanupPrompt = answer;
			_settingsService.SaveSettings();
		}

		return ConfigMenu.MainMenu;
	}
}
