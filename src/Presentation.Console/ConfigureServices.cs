using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Tessa.Presentation.Console.Logging;

namespace Tessa.Presentation.Console;

public static class ConfigureServices
{
	public static IServiceCollection AddPresentationConsoleServices(this IServiceCollection services)
	{
		Log.Logger = new LoggerConfiguration()
			.WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Day)
			.MinimumLevel.Information()
			.CreateLogger();

		//.MinimumLevel.ControlledBy(LogInterceptor.LogLevel)
		//.Enrich.With<LoggingEnricher>()
		//.WriteTo.Map(LoggingEnricher.LogFilePathPropertyName, (logFilePath, config) => config.File($"{logFilePath}"), 1)

		return services
			.AddLogging(configure => configure.AddSerilog(Log.Logger))
			.AddSerilog();
	}
}
