namespace StartMenuCleaner
{
    using System;
    using System.IO;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;

    public static class Logging
	{
		private const LogEventLevel defaultLogLevel = LogEventLevel.Information;

		private static readonly LoggingLevelSwitch loggingSwitch = new LoggingLevelSwitch(defaultLogLevel);

		public static void Startup()
		{
			// Setup the logging.
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.ControlledBy(loggingSwitch)
				.WriteTo.Console(
					outputTemplate: "{Message}{NewLine}{Exception}",
					restrictedToMinimumLevel: LogEventLevel.Information)
				.WriteTo.Trace(restrictedToMinimumLevel: LogEventLevel.Verbose)
				.WriteTo.RollingFile(
					Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs\log-{Date}.txt"),
					retainedFileCountLimit: 14,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
					restrictedToMinimumLevel: LogEventLevel.Debug)
				.CreateLogger();
		}

		public static void SetMinLogLevel(LogEventLevel level = defaultLogLevel)
		{
			loggingSwitch.MinimumLevel = level;
		}
	}
}