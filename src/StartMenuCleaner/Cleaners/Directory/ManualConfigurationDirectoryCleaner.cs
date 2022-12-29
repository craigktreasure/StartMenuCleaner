namespace StartMenuCleaner.Cleaners.Directory;

using StartMenuCleaner.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

internal class ManualConfigurationDirectoryCleaner : DirectoryCleanerBase
{
    private readonly IDictionary<string, ManualDirectoryRemoveConfiguration> configurations;

    private readonly FileSystemOperationHandler fileSystemOperationHandler;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public override CleanerType CleanerType => CleanerType.ManualConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualConfigurationDirectoryCleaner"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="fileSystemOperationHandler">The file system operation handler.</param>
    /// <param name="configurationLoader">The configuration loader.</param>
    public ManualConfigurationDirectoryCleaner(
        IFileSystem fileSystem,
        FileSystemOperationHandler fileSystemOperationHandler,
        IManualConfigurationLoader configurationLoader)
        : base(fileSystem)
    {
        this.configurations = configurationLoader.LoadDirectoryConfigurations();
        this.fileSystemOperationHandler = fileSystemOperationHandler;
    }

    /// <summary>
    /// Determines if the cleaner can clean the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><c>true</c> if the cleaner can clean the directory path, <c>false</c> otherwise.</returns>
    public override bool CanClean(string directoryPath) => this.CanClean(directoryPath, out _);

    /// <summary>
    /// Cleans the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    public override void Clean(string directoryPath)
    {
        if (!this.CanClean(directoryPath, out ManualDirectoryRemoveConfiguration? configuration))
        {
            throw new InvalidOperationException($"The directory cannot be cleaned by the {nameof(ManualConfigurationDirectoryCleaner)}: '{directoryPath}'.");
        }

        string programRootDir = this.FileSystem.Path.GetDirectoryName(directoryPath)!;

        // Promote files if any.
        IEnumerable<string> filesPresentToPromote = this.FileSystem.Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories)
            .Where(filePath => configuration.FilesToPromote.Contains(this.FileSystem.Path.GetRelativePath(directoryPath, filePath).Replace('\\', '/')));

        foreach (string fileToPromote in filesPresentToPromote)
        {
            this.fileSystemOperationHandler.MoveFileToDirectory(programRootDir, fileToPromote, replaceExisting: true);
        }

        // Delete the remaining folder.
        this.fileSystemOperationHandler.DeleteDirectory(directoryPath, recurse: true);
    }

    private bool CanClean(string directoryPath, [NotNullWhen(true)] out ManualDirectoryRemoveConfiguration? configuration)
    {
        this.ValidateDirectory(directoryPath);

        string directoryName = this.FileSystem.Path.GetFileName(directoryPath)
            ?? throw new InvalidOperationException();

        return this.configurations.TryGetValue(directoryName, out configuration);
    }
}
