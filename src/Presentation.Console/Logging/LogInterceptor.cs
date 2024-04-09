using Serilog.Core;
using Spectre.Console.Cli;
using Tessa.Presentation.Console.Commands;

namespace Tessa.Presentation.Console.Logging;

public class LogInterceptor : ICommandInterceptor
{
    public static readonly LoggingLevelSwitch LogLevel = new();

    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is LogCommandSettings logSettings)
        {
            LoggingEnricher.Path = logSettings.LogFile ?? "log.txt";
            LogLevel.MinimumLevel = logSettings.LogLevel;
        }
    }
}
