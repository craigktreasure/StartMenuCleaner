namespace StartMenuCleaner.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;

    public static class StartMenuHelper
	{
		private const string programsFolderName = "Programs";

		public static IEnumerable<string> GetStartMenuProgramDirectories(IFileSystem fileSystem)
		{
			string[] dirs = new string[] {
				Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
				Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
			};

			return dirs.Select(x => fileSystem.Path.Combine(x, programsFolderName))
					   .Where(fileSystem.Directory.Exists);
		}
	}
}