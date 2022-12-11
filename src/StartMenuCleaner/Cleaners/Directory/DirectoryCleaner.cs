namespace StartMenuCleaner.Cleaners.Directory;

using System.IO.Abstractions;

internal class DirectoryCleaner
{
    private readonly IEnumerable<IDirectoryCleaner> cleaners;

    private readonly IFileSystem fileSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryCleaner" /> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="cleaners">The cleaners.</param>
    public DirectoryCleaner(IFileSystem fileSystem, IEnumerable<IDirectoryCleaner> cleaners)
    {
        this.fileSystem = fileSystem;
        this.cleaners = cleaners;
    }

    /// <summary>
    /// Gets the items to clean.
    /// </summary>
    /// <param name="directoryPaths">The directory paths.</param>
    /// <returns><see cref="IReadOnlyList{ProgramDirectoryItem}"/>.</returns>
    public IReadOnlyList<DirectoryItemToClean> GetItemsToClean(IEnumerable<string> directoryPaths)
        => directoryPaths
        .Where(this.fileSystem.Directory.Exists)
        .SelectMany(this.GetItemsToClean)
        .ToArray();

    /// <summary>
    /// Gets the items to clean.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><see cref="IReadOnlyList{ProgramFileItem}"/>.</returns>
    public IReadOnlyList<DirectoryItemToClean> GetItemsToClean(string directoryPath)
    {
        if (!this.fileSystem.Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        List<DirectoryItemToClean> items = new();

        foreach (string subdirectoryPath in this.fileSystem.Directory.GetDirectories(directoryPath))
        {
            foreach (IDirectoryCleaner cleaner in this.cleaners)
            {
                if (cleaner.CanClean(subdirectoryPath))
                {
                    items.Add(new DirectoryItemToClean(subdirectoryPath, cleaner));
                    break;
                }
            }
        }

        return items;
    }
}
