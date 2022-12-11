namespace StartMenuCleaner.Cleaners.Directory;

using StartMenuCleaner.Extensions;
using System.IO.Abstractions;

internal class EmptyDirectoryCleaner : DirectoryCleanerBase
{
    private readonly FileSystemOperationHandler fileSystemOperationHandler;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public override CleanerType CleanerType => CleanerType.EmptyDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyDirectoryCleaner"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="fileSystemOperationHandler">The file system operation handler.</param>
    public EmptyDirectoryCleaner(IFileSystem fileSystem, FileSystemOperationHandler fileSystemOperationHandler)
        : base(fileSystem)
    {
        this.fileSystemOperationHandler = fileSystemOperationHandler;
    }

    /// <summary>
    /// Determines if the cleaner can clean the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><c>true</c> if the cleaner can clean the directory path, <c>false</c> otherwise.</returns>
    public override bool CanClean(string directoryPath)
    {
        if (this.ShouldIgnoreDirectory(directoryPath))
        {
            return false;
        }

        return this.FileSystem.Directory.IsEmpty(directoryPath);
    }

    /// <summary>
    /// Cleans the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    public override void Clean(string directoryPath)
    {
        if (!this.CanClean(directoryPath))
        {
            throw new InvalidOperationException($"The directory cannot be cleaned by the {nameof(EmptyDirectoryCleaner)}: '{directoryPath}'.");
        }

        this.fileSystemOperationHandler.DeleteDirectory(directoryPath);
    }
}
