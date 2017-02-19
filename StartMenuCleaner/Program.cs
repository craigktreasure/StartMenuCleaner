using System;
using System.Linq;
using CommandLine;
using NLog;

namespace StartMenuCleaner
{
	internal class Program
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private static void Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<ProgramOptions>(args);
			if (result.Errors.Any())
			{
				Console.WriteLine("Invalid command line arguments.");
			}

			logger.Info("Starting");
			Console.WriteLine();
			Cleaner cleaner = new Cleaner(result.Value.Simulate);
			cleaner.Start();

			Console.WriteLine();
			logger.Info("Finished");
		}
	}
}