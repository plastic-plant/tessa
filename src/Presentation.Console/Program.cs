using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Tessa.Application;
using Tessa.Infrastructure;
using Tessa.Presentation.Cli.Commands;
using Tessa.Presentation.Cli.Configuration;

var services = new ServiceCollection();
services.AddApplicationServices();
services.AddInfrastructureServices();


var registrar = new ServiceTypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
	config.SetApplicationName("tessa");
	config
		.AddCommand<OcrCommand>("ocr")
		.WithExample("ocr")
		.WithExample("ocr", "--in folder", "--out folder")
		.WithExample("ocr", "--in file.png", "--out file.txt")
		.WithExample("ocr", "--settings tessa.settings.json")
		.WithExample("ocr", "--lang eng")
		.WithExample("ocr", "--help");
});


return await app.RunAsync(args);