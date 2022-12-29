namespace StartMenuCleaner;

using DotNetConfig;
using StartMenuCleaner.Utils;

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

    /// <summary>
    /// Loads the <see cref="CleanerOptions" />.
    /// </summary>
    /// <param name="simulate">if set to <c>true</c> [simulate].</param>
    /// <param name="configurationPath">The configuration path.</param>
    /// <returns><see cref="CleanerOptions"/>.</returns>
    public static CleanerOptions Load(bool simulate = false, string? configurationPath = null)
    {
        Config config = Config.Build(configurationPath);

        IReadOnlyList<string> rootFoldersToClean = StartMenuHelper.GetKnownStartMenuProgramsFolders();
        IReadOnlyList<string> foldersToIgnoreFromConfig = LoadFoldersToIgnoreFromConfiguration(config);
        IReadOnlyList<string> foldersToIgnore = Constants.DirectoriesToIgnore.Concat(foldersToIgnoreFromConfig).ToArray();

        return new(rootFoldersToClean, foldersToIgnore)
        {
            Simulate = simulate,
        };
    }

    private static IReadOnlyList<string> LoadFoldersToIgnoreFromConfiguration(Config config)
        => config.GetAll(Constants.ConfigurationSectionName, "ignore").Select(x => x.GetString()).ToArray();
}
