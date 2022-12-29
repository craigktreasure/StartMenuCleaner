namespace StartMenuCleaner.Cleaners;

internal class ItemToClean
{
    private readonly ICleaner cleaner;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public CleanerType CleanerType => this.cleaner.CleanerType;

    /// <summary>
    /// Gets the item path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemToClean"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="cleaner">The cleaner.</param>
    public ItemToClean(string path, ICleaner cleaner)
    {
        this.Path = path;
        this.cleaner = cleaner;
    }

    /// <summary>
    /// Cleans this item.
    /// </summary>
    public void Clean() => this.cleaner.Clean(this.Path);
}
