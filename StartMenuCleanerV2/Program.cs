using System;
using System.Linq;
using CommandLine;
using Serilog;

namespace StartMenuCleaner
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Logging.Startup();

			Console.Title = "Start Menu Cleaner";

			ParserResult<ProgramOptions> result = Parser.Default.ParseArguments<ProgramOptions>(args);
			if (result.Errors.Any())
			{
				Console.WriteLine("Invalid command line arguments.");
			}

			ProgramOptions options = result.Value;

			Log.Information("Starting");

			if (options.Debug)
			{
				Logging.SetMinLogLevel(Serilog.Events.LogEventLevel.Debug);
				Log.Information("Debug logging is enabled");
			}

			Console.WriteLine();
			Cleaner cleaner = new Cleaner(options.Simulate);
			cleaner.Start();

			Console.WriteLine();
			Log.Information("Finished");

			if (!options.Silent)
			{
				Console.ReadLine();
			}
		}
	}
}