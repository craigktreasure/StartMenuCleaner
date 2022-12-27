namespace StartMenuCleaner;

using StartMenuCleaner.Cleaners.Directory;

internal class CleanerOptions
{
    /// <summary>
    /// Gets the folders to ignore.
    /// </summary>
    public IReadOnlySet<string> FoldersToIgnore { get; }

    /// <summary>
    /// Gets the root folders to clean.
    /// </summary>
    public IReadOnlyList<string> RootFoldersToClean { get; }

    /// <summary>
    /// Gets a value indicating whether to simulate.
    /// </summary>
    public bool Simulate { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CleanerOptions" /> class.
    /// </summary>
    /// <param name="rootFoldersToClean">The contents of these foldres will be searched and cleaned.</param>
    public CleanerOptions(IReadOnlyList<string> rootFoldersToClean)
        : this(rootFoldersToClean, Constants.DirectoriesToIgnore)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CleanerOptions" /> class.
    /// </summary>
    /// <param name="rootFoldersToClean">The contents of these foldres will be searched and cleaned.</param>
    /// <param name="foldersToIgnore">The folders to ignore.</param>
    public CleanerOptions(IReadOnlyList<string> rootFoldersToClean, IReadOnlyList<string> foldersToIgnore)
    {
        this.RootFoldersToClean = Argument.NotNull(rootFoldersToClean);
        this.FoldersToIgnore = new HashSet<string>(Argument.NotNull(foldersToIgnore), StringComparer.CurrentCultureIgnoreCase);
    }
}
