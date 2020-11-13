namespace StartMenuCleaner.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class StartMenuHelper
	{
		private const string programsFolderName = "Programs";

		public static IEnumerable<string> GetStartMenuProgramDirectories()
		{
			string[] dirs = new string[] {
				Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
				Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
			};

			return dirs.Select(x => Path.Combine(x, programsFolderName))
					   .Where(Directory.Exists);
		}
	}
}