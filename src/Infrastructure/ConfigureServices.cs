using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Tessa.Application.Interfaces;
using Tessa.Infrastructure.Repositories;
using Tessa.Infrastructure.Tesseract;

namespace Tessa.Infrastructure;

public static class ConfigureServices
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		services.AddSingleton<IFileRepository, FileRepository>();
		services.AddSingleton<ILlamaRepository, LlamaRepository>();
		services.AddSingleton<ITesseractOcrRepository, TesseractRepository>();
		services.AddSingleton<IFlorenceOcrRepository, FlorenceRepository>();
		services.AddSingleton<IOpenAIRepository, OpenAIRepository>();

		return services;
	}
}
