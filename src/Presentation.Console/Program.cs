using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Tessa.Application;
using Tessa.Infrastructure;
using Tessa.Presentation.Console;
using Tessa.Presentation.Console.Commands;
using Tessa.Presentation.Console.Configuration;

var services = new ServiceCollection()
	.AddPresentationConsoleServices()
	.AddApplicationServices()
	.AddInfrastructureServices();

var registrar = new ServiceTypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
	config.SetApplicationName("tessa");
	// config.SetInterceptor(new LogInterceptor());

	config
		.AddCommand<ConfigCommand>("config")
		.WithDescription("Configures tessa.settings.json.")
		.WithExample("config")
		.WithExample("config", "--settings tessa.settings.json");

	config
		.AddCommand<DownloadCommand>("download")
		.WithDescription("Downloads language models for OCR and LLM prompting.")
		.WithExample("download", "llm")
		.WithExample("download", "llm", "https://huggingface.co/...gguf")
		.WithExample("download", "tessdata")
		.WithExample("download", "tessdata", "ita")
		.WithExample("download", "tessdata", "eng+nld+deu")
		.WithExample("download", "tessdata", "ita")
		.WithExample("download", "tessdata", "https://raw.githubusercontent.com/...traineddata");

	config
		.AddCommand<OcrCommand>("ocr")
		.WithDescription("Reads text from images and documents.")
		.WithExample("ocr")
		.WithExample("ocr", "--in folder", "--out folder")
		.WithExample("ocr", "--in file.png", "--out file.txt")
		.WithExample("ocr", "--settings tessa.settings.json")
		.WithExample("ocr", "--lang eng")
		.WithExample("ocr", "--help");
});

return await app.RunAsync(args);