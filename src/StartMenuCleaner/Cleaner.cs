namespace StartMenuCleaner;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;

using Microsoft.Extensions.Logging;

using StartMenuCleaner.Cleaners;
using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.Cleaners.File;

internal class Cleaner
{
    private readonly IEnumerable<IDirectoryCleaner> directoryCleaners;

    private readonly IEnumerable<IFileCleaner> fileCleaners;

    private readonly IFileSystem fileSystem;

    private readonly ILogger<Cleaner> logger;

    private readonly CleanerOptions options;

    public Cleaner(
        CleanerOptions options,
        ILogger<Cleaner> logger,
        IFileSystem fileSystem,
        IEnumerable<IFileCleaner> fileCleaners,
        IEnumerable<IDirectoryCleaner> directoryCleaners)
    {
        this.options = options;
        this.fileSystem = fileSystem;
        this.fileCleaners = fileCleaners;
        this.directoryCleaners = directoryCleaners;
        this.logger = logger;
    }

    public void Start()
    {
        if (this.options.Simulate)
        {
            this.logger.Simulating();
            Console.WriteLine();
        }

        IReadOnlyList<ItemToClean> itemsToClean = this.GetItemsToClean(this.options.RootFoldersToClean);

        if (itemsToClean.Count == 0)
        {
            this.logger.NothingToClean();
            return;
        }

        this.logger.FoundItemsToClean(itemsToClean);

        this.logger.CleaningStarted();
        this.CleanItems(itemsToClean);

        this.logger.CleaningFinished();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    private void CleanItems(IEnumerable<ItemToClean> itemsToClean)
    {
        foreach (ItemToClean item in itemsToClean)
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

    /// <summary>
    /// Gets the directory items to clean.
    /// </summary>
    /// <param name="directoryPaths">The directory paths.</param>
    /// <returns>An array of <see cref="ItemToClean"/>.</returns>
    private ItemToClean[] GetDirectoryItemsToClean(IEnumerable<string> directoryPaths)
        => directoryPaths
        .Where(this.fileSystem.Directory.Exists)
        .SelectMany(this.GetDirectoryItemsToClean)
        .ToArray();

    /// <summary>
    /// Gets the directory items to clean.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><see cref="IReadOnlyList{ItemToClean}"/>.</returns>
    private IReadOnlyList<ItemToClean> GetDirectoryItemsToClean(string directoryPath)
    {
        if (!this.fileSystem.Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        List<ItemToClean> items = [];

        foreach (string subdirectoryPath in this.fileSystem.Directory.GetDirectories(directoryPath))
        {
            string subdirectoryName = this.fileSystem.Path.GetFileName(subdirectoryPath);

            if (this.options.FoldersToIgnore.Contains(subdirectoryName))
            {
                continue;
            }

            foreach (IDirectoryCleaner cleaner in this.directoryCleaners)
            {
                if (cleaner.CanClean(subdirectoryPath))
                {
                    items.Add(new ItemToClean(subdirectoryPath, cleaner));
                    break;
                }
            }
        }

        return items;
    }

    /// <summary>
    /// Gets the file items to clean.
    /// </summary>
    /// <param name="directoryPaths">The directory paths.</param>
    /// <returns>An array of <see cref="ItemToClean"/>.</returns>
    private ItemToClean[] GetFileItemsToClean(IEnumerable<string> directoryPaths)
        => directoryPaths
        .Where(this.fileSystem.Directory.Exists)
        .SelectMany(this.GetFileItemsToClean)
        .ToArray();

    /// <summary>
    /// Gets the file items to clean.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><see cref="IReadOnlyList{ItemToClean}"/>.</returns>
    private IReadOnlyList<ItemToClean> GetFileItemsToClean(string directoryPath)
    {
        if (!this.fileSystem.Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        List<ItemToClean> items = [];

        foreach (string filePath in this.fileSystem.Directory.GetFiles(directoryPath))
        {
            foreach (IFileCleaner cleaner in this.fileCleaners)
            {
                if (cleaner.CanClean(filePath))
                {
                    items.Add(new ItemToClean(filePath, cleaner));
                    break;
                }
            }
        }

        return items;
    }

    private ItemToClean[] GetItemsToClean(IEnumerable<string> directoryPaths)
        => [.. this.GetFileItemsToClean(directoryPaths), .. this.GetDirectoryItemsToClean(directoryPaths)];
}
