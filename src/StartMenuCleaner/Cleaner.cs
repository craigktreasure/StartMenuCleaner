namespace StartMenuCleaner;

using Microsoft.Extensions.Logging;
using StartMenuCleaner.Cleaners.File;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using systemIO = System.IO;

internal class Cleaner
{
    private readonly CleanupRulesEngine cleanupEngine;

    private readonly FileClassifier fileClassifier;

    private readonly FileCleaner fileCleaner;

    private readonly IFileSystem fileSystem;

    private readonly FileSystemOperationHandler fileSystemOperationHandler;

    private readonly ILogger<Cleaner> logger;

    private readonly CleanerOptions options;

    public Cleaner(
        CleanerOptions options,
        IFileSystem fileSystem,
        FileClassifier fileClassifier,
        FileSystemOperationHandler fileSystemOperationHandler,
        CleanupRulesEngine cleanupEngine,
        FileCleaner fileCleaner,
        ILogger<Cleaner> logger)
    {
        this.options = options;
        this.fileSystem = fileSystem;
        this.fileClassifier = fileClassifier;
        this.fileSystemOperationHandler = fileSystemOperationHandler;
        this.logger = logger;
        this.cleanupEngine = cleanupEngine;
        this.fileCleaner = fileCleaner;
    }

    public void Start()
    {
        if (this.options.Simulate)
        {
            this.logger.Simulating();
            Console.WriteLine();
        }

        IReadOnlyList<ProgramDirectoryItem> directoryItemsToClean = this.GetFoldersToClean()
            .Select(x => new ProgramDirectoryItem(x, this.cleanupEngine.TestForCleanReason(x)))
            .Where(x => x.Reason != CleanReason.None)
            .ToArray();

        IReadOnlyList<ProgramFileItem> fileItemsToClean = this.fileCleaner.GetItemsToClean(this.options.RootFoldersToClean);

        if (directoryItemsToClean.Count == 0 && fileItemsToClean.Count == 0)
        {
            this.logger.NothingToClean();
            return;
        }

        this.logger.FoundItemsToClean(directoryItemsToClean);
        this.logger.FoundItemsToClean(fileItemsToClean);

        this.logger.CleaningStarted();
        this.CleanItems(directoryItemsToClean);
        this.CleanItems(fileItemsToClean);

        this.logger.CleaningFinished();
    }

    private void CleanEmptyDirectory(ProgramDirectoryItem itemToClean)
    {
        if (CleanReason.Empty != itemToClean.Reason)
        {
            throw new ArgumentException($"The item {nameof(itemToClean.Reason)} is not {CleanReason.Empty}.", nameof(itemToClean));
        }

        Func<string, bool> testFunction = this.cleanupEngine.GetReasonTestFunction(CleanReason.Empty);
        if (!testFunction(itemToClean.Path))
        {
            throw new InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
        }

        // Delete the empty folder.
        this.fileSystemOperationHandler.DeleteDirectory(itemToClean.Path);
    }

    private void CleanFewAppsWithCruft(ProgramDirectoryItem itemToClean)
    {
        if (CleanReason.FewAppsWithCruft != itemToClean.Reason)
        {
            throw new ArgumentException($"The item {nameof(itemToClean.Reason)} is not {CleanReason.FewAppsWithCruft}.", nameof(itemToClean));
        }

        Func<string, bool> testFunction = this.cleanupEngine.GetReasonTestFunction(CleanReason.FewAppsWithCruft);
        if (!testFunction(itemToClean.Path))
        {
            throw new InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
        }

        string programRootDir = this.fileSystem.Path.GetDirectoryName(itemToClean.Path)
            ?? throw new InvalidOperationException($"The directory could not be determined: '{itemToClean.Path}'.");

        IEnumerable<FileClassificationItem> files = this.fileSystem.Directory.EnumerateFiles(itemToClean.Path)
            .Select(x => new FileClassificationItem(x, this.fileClassifier.ClassifyFile(x)));

        // Move the app items to the program root directory.
        IEnumerable<string> appFilePaths = files.Where(x => x.Classification == FileClassification.App).Select(x => x.Path);
        this.fileSystemOperationHandler.MoveFilesToDirectory(programRootDir, appFilePaths, replaceExisting: true);

        // Delete the rest of the files.
        IEnumerable<string> otherFilePaths = files.Where(x => x.Classification != FileClassification.App).Select(x => x.Path);
        this.fileSystemOperationHandler.DeleteFiles(otherFilePaths);

        // Delete the empty folder.
        this.fileSystemOperationHandler.DeleteDirectory(itemToClean.Path);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    private void CleanItem(ProgramDirectoryItem itemToClean)
    {
        Action<ProgramDirectoryItem> cleanFunction = this.GetCleanFunction(itemToClean.Reason);

        try
        {
            cleanFunction(itemToClean);
        }
        catch (Exception ex)
        {
            this.logger.CleaningFailed(itemToClean.Path, ex);
        }
    }

    private void CleanItems(IEnumerable<ProgramDirectoryItem> itemsToClean)
    {
        foreach (ProgramDirectoryItem item in itemsToClean)
        {
            this.logger.CleaningItem(item);
            this.CleanItem(item);
        }
    }

    private void CleanItems(IEnumerable<ProgramFileItem> itemsToClean)
    {
        foreach (ProgramFileItem item in itemsToClean)
        {
            this.logger.CleaningItem(item);
            item.Clean();
        }
    }

    private void CleanSingleApp(ProgramDirectoryItem itemToClean)
    {
        if (CleanReason.SingleApp != itemToClean.Reason)
        {
            throw new ArgumentException($"The item {nameof(itemToClean.Reason)} is not {CleanReason.SingleApp}.", nameof(itemToClean));
        }

        Func<string, bool> testFunction = this.cleanupEngine.GetReasonTestFunction(CleanReason.SingleApp);
        if (!testFunction(itemToClean.Path))
        {
            throw new systemIO.InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
        }

        string programRootDir = this.fileSystem.Path.GetDirectoryName(itemToClean.Path)!;

        // Move the only file into the program root directory.
        string currentFileLocation = this.fileSystem.Directory.GetFiles(itemToClean.Path).First();
        this.fileSystemOperationHandler.MoveFileToDirectory(programRootDir, currentFileLocation, replaceExisting: true);

        // Delete the empty folder.
        this.fileSystemOperationHandler.DeleteDirectory(itemToClean.Path);
    }

    private Action<ProgramDirectoryItem> GetCleanFunction(CleanReason reason)
    {
        return reason switch
        {
            CleanReason.Empty => this.CleanEmptyDirectory,
            CleanReason.SingleApp => this.CleanSingleApp,
            CleanReason.FewAppsWithCruft => this.CleanFewAppsWithCruft,
            _ => throw new ArgumentException("No cleanup function was available.", nameof(reason)),
        };
    }

    private IEnumerable<string> GetFoldersToClean() =>
        this.options.RootFoldersToClean
            .Where(this.fileSystem.Directory.Exists)
            .SelectMany(this.fileSystem.Directory.GetDirectories);
}
