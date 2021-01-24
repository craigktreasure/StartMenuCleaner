namespace StartMenuCleaner.Utils
{
    public class FileShortcut
    {
        /// <summary>
        /// Gets the path to the shortcut file.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the path the shortcut points to.
        /// </summary>
        public string Target { get; }

        public FileShortcut(string path, IFileShortcutHandler shortcutResolver)
        {
            this.Path = path;
            this.Target = shortcutResolver.ResolveTarget(path);
        }
    }
}
