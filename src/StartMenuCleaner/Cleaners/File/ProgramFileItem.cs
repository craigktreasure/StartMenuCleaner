namespace StartMenuCleaner.Cleaners.File;

internal class ProgramFileItem
{
    private readonly IFileCleaner cleaner;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public FileCleanerType CleanerType => this.cleaner.CleanerType;

    /// <summary>
    /// Gets the item path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramFileItem"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="cleaner">The cleaner.</param>
    public ProgramFileItem(string path, IFileCleaner cleaner)
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
