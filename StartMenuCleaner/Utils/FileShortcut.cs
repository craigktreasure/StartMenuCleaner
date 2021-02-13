namespace StartMenuCleaner.Utils
{
    public class FileShortcut
    {
        /// <summary>
        /// Gets the path to the shortcut file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the target path the shortcut points to.
        /// </summary>
        public string TargetPath { get; }

        public FileShortcut(string filePath, string targetPath)
        {
            this.FilePath = filePath;
            this.TargetPath = targetPath;
        }
    }
}
