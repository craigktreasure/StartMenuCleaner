namespace StartMenuCleaner.Cleaners.File;

using System.IO.Abstractions;

internal class FileCleaner
{
    private readonly IEnumerable<IFileCleaner> cleaners;

    private readonly IFileSystem fileSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCleaner" /> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="cleaners">The cleaners.</param>
    public FileCleaner(IFileSystem fileSystem, IEnumerable<IFileCleaner> cleaners)
    {
        this.fileSystem = fileSystem;
        this.cleaners = cleaners;
    }

    /// <summary>
    /// Gets the items to clean.
    /// </summary>
    /// <param name="directoryPaths">The directory paths.</param>
    /// <returns><see cref="IReadOnlyList{ProgramFileItem}"/>.</returns>
    public IReadOnlyList<FileItemToClean> GetItemsToClean(IEnumerable<string> directoryPaths)
        => directoryPaths
        .Where(this.fileSystem.Directory.Exists)
        .SelectMany(this.GetItemsToClean)
        .ToArray();

    /// <summary>
    /// Gets the items to clean.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><see cref="IReadOnlyList{ProgramFileItem}"/>.</returns>
    public IReadOnlyList<FileItemToClean> GetItemsToClean(string directoryPath)
    {
        if (!this.fileSystem.Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        List<FileItemToClean> items = new();

        foreach (string filePath in this.fileSystem.Directory.GetFiles(directoryPath))
        {
            foreach (IFileCleaner cleaner in this.cleaners)
            {
                if (cleaner.CanClean(filePath))
                {
                    items.Add(new FileItemToClean(filePath, cleaner));
                    break;
                }
            }
        }

        return items;
    }
}
