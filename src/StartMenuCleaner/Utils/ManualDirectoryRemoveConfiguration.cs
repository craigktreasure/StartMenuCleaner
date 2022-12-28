namespace StartMenuCleaner.Utils;

internal sealed class ManualDirectoryRemoveConfiguration
{
    /// <summary>
    /// Gets the files to keep and promote.
    /// </summary>
    public IReadOnlySet<string> FilesToPromote { get; }

    /// <summary>
    /// Gets the name of the directory to be removed.
    /// </summary>
    public string DirectoryName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualDirectoryRemoveConfiguration"/> class.
    /// </summary>
    /// <param name="directoryName">The name of the directory to be removed.</param>
    /// <param name="filesToPromote">The files to keep and promote.</param>
    public ManualDirectoryRemoveConfiguration(string directoryName, IEnumerable<string> filesToPromote)
    {
        this.DirectoryName = Argument.NotNullOrWhiteSpace(directoryName);
        Argument.NotNull(filesToPromote);
        this.FilesToPromote = new HashSet<string>(filesToPromote, StringComparer.OrdinalIgnoreCase);
    }
}
