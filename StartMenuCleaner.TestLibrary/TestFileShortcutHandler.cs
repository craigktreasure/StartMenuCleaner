namespace StartMenuCleaner.TestLibrary
{
    using StartMenuCleaner.Utils;
    using System;
    using System.Collections.Generic;

    public class TestFileShortcutHandler : IFileShortcutHandler
    {
        private readonly IDictionary<string, string> shortcutMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFileShortcutHandler"/> class.
        /// </summary>
        public TestFileShortcutHandler()
            : this(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFileShortcutHandler"/> class.
        /// </summary>
        /// <param name="shortcutMap">The shortcut map.</param>
        public TestFileShortcutHandler(IDictionary<string, string> shortcutMap)
            => this.shortcutMap = shortcutMap;

        /// <summary>
        /// Adds the shortcut mapping.
        /// </summary>
        /// <param name="shortcutFilePath">The shortcut file path.</param>
        /// <param name="shortcutTargetPath">The shortcut target path.</param>
        public void AddShortcutMapping(string shortcutFilePath, string shortcutTargetPath)
            => this.shortcutMap.Add(shortcutFilePath, shortcutTargetPath);

        /// <summary>
        /// Adds the shortcut mapping.
        /// </summary>
        /// <param name="shortcut">The shortcut.</param>
        public void AddShortcutMapping(FileShortcut shortcut)
            => this.AddShortcutMapping(shortcut.FilePath, shortcut.TargetPath);

        /// <summary>
        /// Resolves the target path of the specified shortcut.
        /// </summary>
        /// <param name="shortcutPath">The shortcut path.</param>
        /// <returns><see cref="string" />.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string ResolveTarget(string shortcutPath)
        {
            if (this.shortcutMap.TryGetValue(shortcutPath, out string? shortcutTarget))
            {
                return shortcutTarget;
            }

            throw new InvalidOperationException("Unable to resolve shortcut.");
        }
    }
}
