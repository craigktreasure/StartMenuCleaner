namespace StartMenuCleaner.Cleaners.Directory;

using System.IO.Abstractions;

internal class SingleAppDirectoryCleaner : DirectoryCleanerBase
{
    public override CleanerType CleanerType => CleanerType.SingleAppDirectory;

    private readonly FileClassifier fileClassifier;

    private readonly FileSystemOperationHandler fileSystemOperationHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleAppDirectoryCleaner"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="fileSystemOperationHandler">The file system operation handler.</param>
    /// <param name="fileClassifier">The file classifier.</param>
    public SingleAppDirectoryCleaner(
        IFileSystem fileSystem,
        FileSystemOperationHandler fileSystemOperationHandler,
        FileClassifier fileClassifier)
        : base(fileSystem)
    {
        this.fileSystemOperationHandler = fileSystemOperationHandler;
        this.fileClassifier = fileClassifier;
    }

    /// <summary>
    /// Determines if the cleaner can clean the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><c>true</c> if the cleaner can clean the directory path, <c>false</c> otherwise.</returns>
    public override bool CanClean(string directoryPath)
    {
        this.ValidateDirectory(directoryPath);

        if (this.FileSystem.Directory.EnumerateDirectories(directoryPath).Any())
        {
            // If it has directories, it doesn't fit.
            return false;
        }

        IReadOnlyList<string> filePaths = this.FileSystem.Directory
            .EnumerateFiles(directoryPath)
            .ToArray();

        if (filePaths.Count != 1)
        {
            return false;
        }

        string filePath = filePaths[0];

        return this.fileClassifier.ClassifyFile(filePath) == FileClassification.App;
    }

    /// <summary>
    /// Cleans the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    public override void Clean(string directoryPath)
    {
        if (!this.CanClean(directoryPath))
        {
            throw new InvalidOperationException($"The directory cannot be cleaned by the {nameof(SingleAppDirectoryCleaner)}: '{directoryPath}'.");
        }

        string programRootDir = this.FileSystem.Path.GetDirectoryName(directoryPath)!;

        // Move the only file into the program root directory.
        string currentFileLocation = this.FileSystem.Directory.GetFiles(directoryPath).Single();
        this.fileSystemOperationHandler.MoveFileToDirectory(programRootDir, currentFileLocation, replaceExisting: true);

        // Delete the empty folder.
        this.fileSystemOperationHandler.DeleteDirectory(directoryPath);
    }
}
