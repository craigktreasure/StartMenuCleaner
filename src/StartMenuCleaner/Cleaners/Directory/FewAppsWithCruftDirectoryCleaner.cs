namespace StartMenuCleaner.Cleaners.Directory;

using System.IO.Abstractions;

internal class FewAppsWithCruftDirectoryCleaner : DirectoryCleanerBase
{
    private const int minCruftApps = 2;

    private readonly FileClassifier fileClassifier;

    private readonly FileSystemOperationHandler fileSystemOperationHandler;

    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    public override CleanerType CleanerType => CleanerType.FewAppsWithCruftDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FewAppsWithCruftDirectoryCleaner"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="fileSystemOperationHandler">The file system operation handler.</param>
    /// <param name="fileClassifier">The file classifier.</param>
    public FewAppsWithCruftDirectoryCleaner(IFileSystem fileSystem, FileSystemOperationHandler fileSystemOperationHandler, FileClassifier fileClassifier)
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

        IReadOnlyList<string> filePaths = [.. this.FileSystem.Directory.EnumerateFiles(directoryPath)];

        // Classify the files
        IReadOnlyList<FileClassificationItem> classifiedFiles =
            [.. filePaths.Select(x => new FileClassificationItem(x, this.fileClassifier.ClassifyFile(x)))];

        if (classifiedFiles.Any(x => x.Classification == FileClassification.Other))
        {
            // Can't safely cleanup unknown files.
            return false;
        }

        if (classifiedFiles.Count(x => x.Classification == FileClassification.App) > minCruftApps)
        {
            // Should leave this folder intact.
            return false;
        }

        IEnumerable<FileClassificationItem> unremovableFiles = classifiedFiles
            .Where(x => x.Classification != FileClassification.App && !CanBeRemoved(x.Classification));

        return !unremovableFiles.Any();
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

        IEnumerable<FileClassificationItem> files = this.FileSystem.Directory.EnumerateFiles(directoryPath)
            .Select(x => new FileClassificationItem(x, this.fileClassifier.ClassifyFile(x)));

        // Move the app items to the program root directory.
        IEnumerable<string> appFilePaths = files.Where(x => x.Classification == FileClassification.App).Select(x => x.Path);
        this.fileSystemOperationHandler.MoveFilesToDirectory(programRootDir, appFilePaths, replaceExisting: true);

        // Delete the rest of the files.
        IEnumerable<string> otherFilePaths = files.Where(x => x.Classification != FileClassification.App).Select(x => x.Path);
        this.fileSystemOperationHandler.DeleteFiles(otherFilePaths);

        // Delete the empty folder.
        this.fileSystemOperationHandler.DeleteDirectory(directoryPath);
    }

    private static bool CanBeRemoved(FileClassification classification)
        => classification is not FileClassification.App and not FileClassification.Other;
}
