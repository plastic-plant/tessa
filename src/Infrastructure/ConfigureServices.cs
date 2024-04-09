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
		services.AddSingleton<ITesseractRepository, TesseractRepository>();
		services.AddSingleton<IFileRepository, FileRepository>();

		return services;
	}
}
