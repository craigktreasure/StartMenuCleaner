namespace StartMenuCleaner
{
    using CommandLine;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Class is deserialized at runtime.")]
    internal class ProgramOptions
	{
		[Option('s', "simulate", HelpText = "Simulate all file operations.")]
		public bool Simulate { get; set; }

		[Option("silent", HelpText = "Silently run and exit the application.")]
		public bool Silent { get; set; }

		[Option('d', "debug", HelpText = "Enable debug information in console.")]
		public bool Debug { get; set; }
	}
}