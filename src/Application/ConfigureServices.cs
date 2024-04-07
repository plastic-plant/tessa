using Microsoft.Extensions.DependencyInjection;
using Tessa.Application.Interface;
using Tessa.Application.Interfaces;
using Tessa.Application.Services;

namespace Tessa.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IOcrService, OcrService>();
        services.AddSingleton<ISettingsService, SettingsService>();		
	}
}
