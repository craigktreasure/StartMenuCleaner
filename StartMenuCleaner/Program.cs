using System;
using System.Linq;
using CommandLine;
using NLog;

namespace StartMenuCleaner
{
	internal static class Program
	{
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		private static void Main(string[] args)
		{
			ParserResult<ProgramOptions> result = Parser.Default.ParseArguments<ProgramOptions>(args);
			if (result.Errors.Any())
			{
				Console.WriteLine("Invalid command line arguments.");
			}

			ProgramOptions options = result.Value;

			logger.Info("Starting");
			Console.WriteLine();
			Cleaner cleaner = new Cleaner(options.Simulate);
			cleaner.Start();

			Console.WriteLine();
			logger.Info("Finished");

			if (!options.Silent)
			{
				Console.ReadLine();
			}
		}
	}
}