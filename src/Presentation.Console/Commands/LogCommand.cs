using Serilog.Events;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Globalization;

namespace Tessa.Presentation.Console.Commands;

public class LogCommandSettings : CommandSettings
{
    [CommandOption("--logfile")]
    [Description("Path and file name for logging")]
    public string LogFile { get; set; }

    [CommandOption("--loglevel")]
    [Description("Minimum level for logging")]
    [TypeConverter(typeof(VerbosityConverter))]
    [DefaultValue(LogEventLevel.Information)]
    public LogEventLevel LogLevel { get; set; }
}

public sealed class VerbosityConverter : TypeConverter
{
    private readonly Dictionary<string, LogEventLevel> _lookup;

    public VerbosityConverter()
    {
        _lookup = new Dictionary<string, LogEventLevel>(StringComparer.OrdinalIgnoreCase)
            {
                {"d", LogEventLevel.Debug},
                {"v", LogEventLevel.Verbose},
                {"i", LogEventLevel.Information},
                {"w", LogEventLevel.Warning},
                {"e", LogEventLevel.Error},
                {"f", LogEventLevel.Fatal}
            };
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            var result = _lookup.TryGetValue(stringValue, out var verbosity);
            if (!result)
            {
                const string format = "The value '{0}' is not a valid log verbosity.";
                var message = string.Format(CultureInfo.InvariantCulture, format, value);
                throw new InvalidOperationException(message);
            }
            return verbosity;
        }
        throw new NotSupportedException("Can't convert value to log verbosity.");
    }
}
