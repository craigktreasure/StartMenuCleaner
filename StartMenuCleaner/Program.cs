namespace StartMenuCleaner
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using CommandLine;
    using Serilog;

    internal static class Program
	{
		private static void Main(string[] args)
		{
			Logging.Startup();

			Console.Title = "Start Menu Cleaner";

			Parser.Default.ParseArguments<ProgramOptions>(args).WithParsed(Run);
		}

		private static void Run(ProgramOptions options)
		{
            IServiceProvider services = ConfigureServices(options);

            Log.Information("Starting");

			if (options.Debug)
			{
				Logging.SetMinLogLevel(Serilog.Events.LogEventLevel.Debug);
				Log.Information("Debug logging is enabled");
			}

			Console.WriteLine();
            Cleaner cleaner = services.GetRequiredService<Cleaner>();
			cleaner.Start();

			Console.WriteLine();
			Log.Information("Finished");

			if (options.Wait)
			{
				Console.ReadLine();
			}
		}

        private static IServiceProvider ConfigureServices(ProgramOptions options)
        {
            CleanerOptions cleanerOptions = new CleanerOptions
            {
                Simulate = options.Simulate,
            };

            return new ServiceCollection()
                .AddSingleton(cleanerOptions)
                .AddTransient<Cleaner>()
                .BuildServiceProvider();
        }
	}
}