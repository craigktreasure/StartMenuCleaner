namespace StartMenuCleaner.Cleaners.File;

using StartMenuCleaner.Utils;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

internal class ManualConfigurationFileCleaner : IFileCleaner
{
    private readonly IDictionary<string, ManualFileRemoveConfiguration> configurations;

    private readonly IFileSystem fileSystem;

    private readonly FileSystemOperationHandler fileSystemOperationHandler;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public CleanerType CleanerType => CleanerType.ManualConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualConfigurationFileCleaner"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="fileSystemOperationHandler">The file system operation handler.</param>
    /// <param name="configurationLoader">The configuration loader.</param>
    public ManualConfigurationFileCleaner(
        IFileSystem fileSystem,
        FileSystemOperationHandler fileSystemOperationHandler,
        IManualConfigurationLoader configurationLoader)
    {
        this.fileSystem = fileSystem;
        this.fileSystemOperationHandler = fileSystemOperationHandler;
        this.configurations = configurationLoader.LoadFileConfigurations();
    }

    /// <summary>
    /// Determines if the cleaner can clean the specified item path.
    /// </summary>
    /// <param name="itemPath">The item path.</param>
    /// <returns><c>true</c> if the cleaner can clean the item path, <c>false</c> otherwise.</returns>
    public bool CanClean(string itemPath) => this.CanClean(itemPath, out _);

    /// <summary>
    /// Cleans the specified item path.
    /// </summary>
    /// <param name="itemPath">The item path.</param>
    public void Clean(string itemPath)
    {
        if (!this.CanClean(itemPath, out ManualFileRemoveConfiguration? configuration))
        {
            throw new InvalidOperationException($"The file cannot be cleaned by the {nameof(ManualConfigurationFileCleaner)}: '{itemPath}'.");
        }

        this.fileSystemOperationHandler.DeleteFile(itemPath);
    }

    private bool CanClean(string itemPath, [NotNullWhen(true)] out ManualFileRemoveConfiguration? configuration)
    {
        if (!this.fileSystem.File.Exists(itemPath))
        {
            throw new FileNotFoundException(itemPath);
        }

        string fileName = this.fileSystem.Path.GetFileName(itemPath)
            ?? throw new InvalidOperationException();

        return this.configurations.TryGetValue(fileName, out configuration);
    }
}
