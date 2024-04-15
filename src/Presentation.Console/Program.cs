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
		.AddCommand<DownloadCommand>("download")
		.WithDescription("Downloads language models for OCR and LLM prompting.")
		.WithExample("download", "tessdata")
		.WithExample("download", "tessdata", "ita")
		.WithExample("download", "tessdata", "eng+nld+deu")
		.WithExample("download", "tessdata", "ita") // 	https://raw.githubusercontent.com/tesseract-ocr/tessdata/main/afr.traineddata
		.WithExample("download", "llm", "https://huggingface.co/NousResearch/Hermes-2-Pro-Mistral-7B-GGUF/blob/main/Hermes-2-Pro-Mistral-7B.Q4_K_M.gguf");

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