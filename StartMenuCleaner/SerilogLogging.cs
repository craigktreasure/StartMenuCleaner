namespace StartMenuCleaner
{
    using System;
    using System.IO;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;

    internal static class SerilogLogging
	{
		private const LogEventLevel defaultLogLevel = LogEventLevel.Information;

		private static readonly LoggingLevelSwitch loggingSwitch = new LoggingLevelSwitch(defaultLogLevel);

		public static void SetMinLogLevel(LogEventLevel level = defaultLogLevel)
		{
			loggingSwitch.MinimumLevel = level;
		}

        public static ILogger Create()
        {
            return new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingSwitch)
                .WriteTo.Console(
                    outputTemplate: "{Message}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Trace(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.File(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs\log-.txt"),
                    retainedFileCountLimit: 14,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
	}
}