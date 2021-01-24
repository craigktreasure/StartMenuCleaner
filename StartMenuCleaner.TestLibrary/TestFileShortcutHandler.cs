namespace StartMenuCleaner.TestLibrary
{
    using StartMenuCleaner.Utils;
    using System;
    using System.Collections.Generic;

    public class TestFileShortcutHandler : IFileShortcutHandler
    {
        private readonly IDictionary<string, string> shortcutMap;

        public TestFileShortcutHandler()
            : this(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)) { }

        public TestFileShortcutHandler(IDictionary<string, string> shortcutMap)
            => this.shortcutMap = shortcutMap;

        public void AddShortcutMapping(string shortcutFilePath, string shortcutTargetPath)
            => this.shortcutMap.Add(shortcutFilePath, shortcutTargetPath);

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
