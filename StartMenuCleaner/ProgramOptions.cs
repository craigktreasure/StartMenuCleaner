namespace StartMenuCleaner
{
    using CommandLine;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Class is deserialized at runtime.")]
    internal class ProgramOptions
	{
        [Option('d', "debug", HelpText = "Enable debug information in console.")]
        public bool Debug { get; set; }

        [Option('s', "simulate", HelpText = "Simulate all file operations.")]
        public bool Simulate { get; set; }

		[Option('w', "wait", HelpText = "Wait before exiting the application.")]
		public bool Wait { get; set; }
	}
}