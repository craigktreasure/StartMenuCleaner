namespace StartMenuCleaner.Cleaners.Directory;

using System.IO.Abstractions;

internal abstract class DirectoryCleanerBase : IDirectoryCleaner
{
    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public abstract DirectoryCleanerType CleanerType { get; }

    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryCleanerBase"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    protected DirectoryCleanerBase(IFileSystem fileSystem)
    {
        this.FileSystem = fileSystem;
    }

    /// <summary>
    /// Determines if the cleaner can clean the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><c>true</c> if the cleaner can clean the directory path, <c>false</c> otherwise.</returns>
    public abstract bool CanClean(string directoryPath);

    /// <summary>
    /// Cleans the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    public abstract void Clean(string directoryPath);

    /// <summary>
    /// Determines if the directory should be ignored.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><c>true</c> if the directory should be ignored, <c>false</c> otherwise.</returns>
    protected bool ShouldIgnoreDirectory(string directoryPath)
    {
        if (!this.FileSystem.Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        string directoryName = this.FileSystem.Path.GetFileName(directoryPath);

        return Constants.DirectoriesToIgnore.Contains(directoryName, StringComparer.CurrentCultureIgnoreCase);
    }
}
