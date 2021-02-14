namespace StartMenuCleaner.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class StartMenuHelper
    {
        private const string programsFolderName = "Programs";

        /// <summary>
        /// Gets the known start menu folders.
        /// </summary>
        /// <returns><see cref="IReadOnlyList{string}"/>.</returns>
        public static IReadOnlyList<string> GetKnownStartMenuFolders() => new[] {
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
        };

        /// <summary>
        /// Gets the known start menu programs folders.
        /// </summary>
        /// <returns><see cref="IEnumerable{string}"/>.</returns>
        public static IReadOnlyList<string> GetKnownStartMenuProgramsFolders() =>
            GetKnownStartMenuFolders()
                .Select(x => System.IO.Path.Combine(x, programsFolderName))
                .ToArray();
    }
}