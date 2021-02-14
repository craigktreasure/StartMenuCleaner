namespace StartMenuCleaner.Utils
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal interface IFileShortcutHandler
    {
        private const string lnkFileExtension = ".lnk";

        /// <summary>
        /// Creates a file shortcut.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns><see cref="FileShortcut"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public FileShortcut CreateShortcut(string filePath)
        {
            if (this.TryCreateShortcut(filePath, out FileShortcut? result))
            {
                return result;
            }

            throw new ArgumentException("The path specified is not a valid file shortcut.", nameof(filePath));
        }

        /// <summary>
        /// Determines whether the specified file path is a shortcut.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns><see cref="bool"/>.</returns>
        public bool IsShortcut(string filePath)
        {
            string ext = System.IO.Path.GetExtension(filePath);

            return ext == lnkFileExtension;
        }

        /// <summary>
        /// Resolves the target path of the specified shortcut.
        /// </summary>
        /// <param name="shortcutPath">The shortcut path.</param>
        /// <returns><see cref="System.String"/>.</returns>
        string ResolveTarget(string shortcutPath);

        /// <summary>
        /// Tries to create a file shortcut.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="shortcut">The shortcut.</param>
        /// <returns><see cref="bool"/>.</returns>
        public bool TryCreateShortcut(string filePath, [NotNullWhen(true)] out FileShortcut? shortcut)
        {
            shortcut = null;

            if (!this.IsShortcut(filePath))
            {
                return false;
            }

            try
            {
                string target = this.ResolveTarget(filePath);

                shortcut = new FileShortcut(filePath, target);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
