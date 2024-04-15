using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Net;
using Tessa.Application.Enums;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Models;
using Tessa.Application.Services;
using Tessa.Presentation.Console.Enums;
using static Tessa.Application.Models.AppRegistry;
using static Tessa.Infrastructure.Repositories.OpenAIModelResponse;

namespace Tessa.Presentation.Console.Commands;

public class DownloadCommand : AsyncCommand<DownloadCommand.Settings>
{
	private readonly ILogger<OcrCommand> _logger;
	private readonly ISettingsService _settingsService;
	private readonly IDownloadService _download;

	public class Settings: LogCommandSettings
	{
		[CommandArgument(0, "<model-type>")]
		public LanguageModelType ModelType { get; set; }

		[CommandArgument(1, "[name-or-url]")]
		public string? NameOrUrl { get; set; }

		[CommandOption("--settings <filename>")]
		[Description("Use an alternative settings file")]
		[DefaultValue(AppSettings.Defaults.SettingsPath)]
		public required string SettingsPath { get; set; }
	}

	public DownloadCommand(ILogger<OcrCommand> logger, ISettingsService settingsService, IDownloadService download)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
		_download = download ?? throw new ArgumentNullException(nameof(download));
	}

	public override ValidationResult Validate(CommandContext context, Settings settings)
	{
		_settingsService.LoadRegistry();
		_settingsService.LoadSettings(settings.SettingsPath);

		return base.Validate(context, settings);
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
	{
		switch (settings.ModelType)
		{
			case LanguageModelType.none:
				break;

			case LanguageModelType.tessdata:
				var downloads = new List<TessdataModel>();
				var choices = new List<string>();
				var isUrlGiven = Uri.TryCreate(settings.NameOrUrl, UriKind.Absolute, out _);
				var isAliasGiven = !isUrlGiven && !string.IsNullOrWhiteSpace(settings.NameOrUrl);

				// Ask the user which models to download from examples list in repository.
				if (!isUrlGiven && !isAliasGiven)
				{
					
					choices = AnsiConsole.Prompt(
						new MultiSelectionPrompt<string>()
							.Title("Which Tesseract [green]language model[/] would you like to download?")
							.NotRequired()
							.PageSize(20)
							.MoreChoicesText("[grey](Move up and down to scroll through more trained models.)[/]")
							.InstructionsText(
								"[grey](Press [blue]<space>[/] to toggle a model, " +
								"[green]<enter>[/] to start download)[/]")
							.AddChoices(_settingsService.Registry.Tessdata.Select(model => model.Alias)));
				}

				// Add choices for a list of given model alias names on command-line (eng+nld+greek)
				if (isAliasGiven)
				{
					var entries = settings.NameOrUrl.Split("+", StringSplitOptions.TrimEntries);
					choices.AddRange(entries);
				}

				// Create a temporary new model for download list if a url is given.
				if (isUrlGiven)
				{
					var name = $"Downloaded {Path.GetRandomFileName()}.tessdata";
					downloads.Add(new()
					{
						Name = name,
						Alias = name,
						Url = settings.NameOrUrl
					});
				}

				// Add models matching given alias names to download list.
				downloads.AddRange(_settingsService.Registry.Tessdata.Where(model => choices.Contains(model.Alias)));

				// Download models requested and update progress control.
				await AnsiConsole.Progress().StartAsync(async context =>
				{
					foreach (var model in downloads)
					{
						var progressControl = context.AddTask($"[green]{model.Alias}[/] [grey]download[/]");
						string destinationPath = Path.Combine(_settingsService.Settings.Ocr.TessdataPath, model.Name);
						var progressCallback = new Action<double>(percent => progressControl.Value = percent);
						await _download.DownloadFileAsync(model.Url, destinationPath, progressCallback);
					}
				});

				return (int)ExitCode.OK;

			case LanguageModelType.llm:
				var isValidUrl = Uri.TryCreate(settings.NameOrUrl, UriKind.Absolute, out _);
				if (!isValidUrl)
				{
					settings.NameOrUrl = AnsiConsole.Ask<string>("What url should I download the large language model?\n[grey]You can find LLMs at website[/] [blue]huggingface.co[/] [grey](.gguf)[/]", "https://huggingface.co/NousResearch/Hermes-2-Pro-Mistral-7B-GGUF/blob/main/Hermes-2-Pro-Mistral-7B.Q4_K_M.gguf");
				}

				await AnsiConsole.Progress().StartAsync(async context =>
				{
					var progressControl = context.AddTask($"[green]Downloading[/]");
					string destinationPath = Path.Combine(_settingsService.Settings.Llm.ModelsPath, $"Downloaded {Path.GetRandomFileName()}.gguf");
					var progressCallback = new Action<double>(percent => progressControl.Value = percent);
					await _download.DownloadFileAsync(settings.NameOrUrl, destinationPath, progressCallback);
				});
				return (int)ExitCode.OK;

			default:
				break;
		}

		return (int)ExitCode.OK;
	}
}
