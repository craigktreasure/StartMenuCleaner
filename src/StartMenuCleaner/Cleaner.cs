namespace StartMenuCleaner;

using Microsoft.Extensions.Logging;
using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.Cleaners.File;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;

internal class Cleaner
{
    private readonly DirectoryCleaner directoryCleaner;

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
        FileCleaner fileCleaner,
        DirectoryCleaner directoryCleaner,
        ILogger<Cleaner> logger)
    {
        this.options = options;
        this.fileSystem = fileSystem;
        this.fileClassifier = fileClassifier;
        this.fileSystemOperationHandler = fileSystemOperationHandler;
        this.fileCleaner = fileCleaner;
        this.directoryCleaner = directoryCleaner;
        this.logger = logger;
    }

    public void Start()
    {
        if (this.options.Simulate)
        {
            this.logger.Simulating();
            Console.WriteLine();
        }

        IReadOnlyList<ProgramFileItem> fileItemsToClean = this.fileCleaner.GetItemsToClean(this.options.RootFoldersToClean);

        IReadOnlyList<DirectoryItemToClean> directoryItemsToClean = this.directoryCleaner.GetItemsToClean(this.options.RootFoldersToClean);

        if (fileItemsToClean.Count == 0
            && directoryItemsToClean.Count == 0)
        {
            this.logger.NothingToClean();
            return;
        }

        this.logger.FoundItemsToClean(fileItemsToClean);
        this.logger.FoundItemsToClean(directoryItemsToClean);

        this.logger.CleaningStarted();
        this.CleanItems(fileItemsToClean);
        this.CleanItems(directoryItemsToClean);

        this.logger.CleaningFinished();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    private void CleanItems(IEnumerable<ProgramFileItem> itemsToClean)
    {
        foreach (ProgramFileItem item in itemsToClean)
        {
            this.logger.CleaningItem(item);

            try
            {
                item.Clean();
            }
            catch (Exception ex)
            {
                this.logger.CleaningFailed(item.Path, ex);
            }
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    private void CleanItems(IEnumerable<DirectoryItemToClean> itemsToClean)
    {
        foreach (DirectoryItemToClean item in itemsToClean)
        {
            this.logger.CleaningItem(item);

            try
            {
                item.Clean();
            }
            catch (Exception ex)
            {
                this.logger.CleaningFailed(item.Path, ex);
            }
        }
    }
}
