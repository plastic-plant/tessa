using Serilog.Core;
using Serilog.Events;

namespace Tessa.Presentation.Console.Logging;

public class LoggingEnricher : ILogEventEnricher
{
    private string _cachedLogFilePath;
    private LogEventProperty _cachedLogFilePathProperty;

    public static string Path = string.Empty; // Set by LogInterceptor.cs // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output", "log.txt")

	public const string LogFilePathPropertyName = "LogFilePath";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        LogEventProperty logFilePathProperty;

        if (_cachedLogFilePathProperty != null && Path.Equals(_cachedLogFilePath))
        {
            logFilePathProperty = _cachedLogFilePathProperty;
        }
        else
        {
            _cachedLogFilePath = Path;
            _cachedLogFilePathProperty = logFilePathProperty = propertyFactory.CreateProperty(LogFilePathPropertyName, Path);
        }

        logEvent.AddPropertyIfAbsent(logFilePathProperty);
    }
}
