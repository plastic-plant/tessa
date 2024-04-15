using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Tessa.Application.Interface;
using Tessa.Application.Models;
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
		return base.Validate(context, settings);
	}

	public override int Execute(CommandContext context, Settings settings)
	{
		var menuChoice = AnsiConsole
			.Prompt(new SelectionPrompt<string>()
			.Title("\nWelcome to [underline green]tessa config[/]. I'm here to help you update settings in [purple]tessa.settings.json[/].")
			.PageSize(10)
			.AddChoices(
				"1. Change the path where [lightskyblue1]log files[/] are written.",
				"2. Change the path where [lightskyblue1]images and documents[/] are read.",
				"3. Change the path where [lightskyblue1]text files[/] after OCR are written.",
				"4. Change the path where [lightskyblue1]Tesseract tessdata[/] are stored.",
				"5. Change the path where [lightskyblue1]Large language models[/] are stored.",
				$"6. Change the [lightskyblue1]OCR engine[/] selected: [green]{_settingsService.Settings.Ocr.Engine}[/]",
				$"7. Change the [lightskyblue1]OCR language[/] models: [green]{_settingsService.Settings.Ocr.TessdataLanguage}[/]",
				$"8. Change the [lightskyblue1]LLM prompting[/] model: [green]{_settingsService.Settings.Ocr.SelectedProviderConfigName}[/]",
				"9. Change the cleanup strategy and [lightskyblue1]prompt to optimize[/] text readouts.",
				"Q. Quit configuration"
			)).ToLower();

		while (menuChoice[0] != 'q')
		{
			switch (menuChoice[0])
			{
				case '1':

					AnsiConsole.MarkupLine($"\nType 0 to go back to menu. Press Enter to save the current configured default.\n\n");
					var logTextPath = new TextPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _settingsService.Settings.LogPath))
						.RootColor(Color.Red)
						.SeparatorColor(Color.Green)
						.StemColor(Color.Blue)
						.LeafColor(Color.Yellow);
					var panel = new Panel(logTextPath);
					panel.Header = new PanelHeader("1. Change the path where [lightskyblue1]log files[/] are written");
					panel.Border = BoxBorder.Rounded;
					panel.Expand = true;
					AnsiConsole.Write(panel);

					var answer = AnsiConsole.Ask<string>("\n\n", _settingsService.Settings.LogPath);
					switch (answer)
					{
						case "0":
							break;

						default:
							break;
					}

					break;
				case '2':
					break;

				case 'Q':
					return (int)ExitCode.OK;

				default:
					break;
			}
		}



		return (int)ExitCode.OK;
	}
}
