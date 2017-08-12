using CommandLine;

namespace StartMenuCleaner
{
	internal class ProgramOptions
	{
		[Option('s', "simulate", HelpText = "Simulate all file operations.")]
		public bool Simulate { get; set; }

		[Option("silent", HelpText = "Silently run and exit the application.")]
		public bool Silent { get; set; }
	}
}