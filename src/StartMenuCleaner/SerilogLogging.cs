namespace StartMenuCleaner;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Globalization;
using System.IO;

internal static class SerilogLogging
{
    private const LogEventLevel defaultLogLevel = LogEventLevel.Information;

    private static readonly LoggingLevelSwitch loggingSwitch = new(defaultLogLevel);

    public static void SetMinLogLevel(LogEventLevel level = defaultLogLevel)
        => loggingSwitch.MinimumLevel = level;

    public static ILogger Create()
    {
        return new LoggerConfiguration()
            .MinimumLevel.ControlledBy(loggingSwitch)
            .WriteTo.Console(
                outputTemplate: "{Message}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information,
                formatProvider: CultureInfo.CurrentCulture)
            .WriteTo.Trace(
                restrictedToMinimumLevel: LogEventLevel.Verbose,
                formatProvider: CultureInfo.CurrentCulture)
            .WriteTo.File(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs\log-.txt"),
                retainedFileCountLimit: 14,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Debug,
                rollingInterval: RollingInterval.Day,
                formatProvider: CultureInfo.CurrentCulture)
            .CreateLogger();
    }
}
