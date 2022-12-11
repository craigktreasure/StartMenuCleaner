namespace StartMenuCleaner.Cleaners.File;

using StartMenuCleaner.Utils;
using System.IO.Abstractions;

internal class BadShortcutFileCleaner : IFileCleaner
{
    private readonly IFileSystem fileSystem;

    private readonly FileSystemOperationHandler fileSystemOperationHandler;

    private readonly IFileShortcutHandler shortcutHandler;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public FileCleanerType CleanerType => FileCleanerType.BadShortcut;

    /// <summary>
    /// Initializes a new instance of the <see cref="BadShortcutFileCleaner" /> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="shortcutHandler">The shortcut handler.</param>
    /// <param name="fileSystemOperationHandler">The file system operation handler.</param>
    public BadShortcutFileCleaner(
        IFileSystem fileSystem,
        IFileShortcutHandler shortcutHandler,
        FileSystemOperationHandler fileSystemOperationHandler)
    {
        this.fileSystem = fileSystem;
        this.shortcutHandler = shortcutHandler;
        this.fileSystemOperationHandler = fileSystemOperationHandler;
    }

    /// <summary>
    /// Determines if the cleaner can clean the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns><c>true</c> if the cleaner can clean the file path, <c>false</c> otherwise.</returns>
    public bool CanClean(string filePath)
    {
        if (!this.fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        return this.shortcutHandler.TryGetShortcut(filePath, out FileShortcut? shortcut)
            && !string.IsNullOrEmpty(shortcut.TargetPath)
            && !this.fileSystem.File.Exists(shortcut.TargetPath);
    }

    /// <summary>
    /// Cleans the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public void Clean(string filePath)
    {
        if (!this.CanClean(filePath))
        {
            throw new InvalidOperationException($"The file cannot be cleaned by the {nameof(BadShortcutFileCleaner)}: '{filePath}'.");
        }

        this.fileSystemOperationHandler.DeleteFile(filePath);
    }
}
