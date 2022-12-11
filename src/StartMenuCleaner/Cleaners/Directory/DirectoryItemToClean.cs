namespace StartMenuCleaner.Cleaners.Directory;

internal class DirectoryItemToClean
{
    private readonly IDirectoryCleaner cleaner;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public DirectoryCleanerType CleanerType => this.cleaner.CleanerType;

    /// <summary>
    /// Gets the item path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryItemToClean"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="cleaner">The cleaner.</param>
    public DirectoryItemToClean(string path, IDirectoryCleaner cleaner)
    {
        this.Path = path;
        this.cleaner = cleaner;
    }

    /// <summary>
    /// Cleans this item.
    /// </summary>
    public void Clean()
    {
        this.cleaner.Clean(this.Path);
    }
}
