namespace StartMenuCleaner
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;
    using systemIO = System.IO;

    internal class Cleaner
    {
        private readonly CleanupRulesEngine cleanupEngine;

        private readonly FileClassifier fileClassifier;

        private readonly IFileSystem fileSystem;

        private readonly ILogger<Cleaner> logger;

        private readonly CleanerOptions options;

        public Cleaner(CleanerOptions options, IFileSystem fileSystem, FileClassifier fileClassifier, CleanupRulesEngine cleanupEngine, ILogger<Cleaner> logger)
        {
            this.options = options;
            this.fileSystem = fileSystem;
            this.fileClassifier = fileClassifier;
            this.logger = logger;
            this.cleanupEngine = cleanupEngine;
        }

        public void Start()
        {
            if (this.options.Simulate)
            {
                this.logger.LogInformation("Simulating. No changes will be made.");
                Console.WriteLine();
            }

            IEnumerable<string> foldersToClean = this.GetFoldersToClean();

            IEnumerable<ProgramDirectoryItem> itemsToClean = foldersToClean
                .Select(x => new ProgramDirectoryItem(x, this.cleanupEngine.TestForCleanReason(x)))
                .Where(x => x.Reason != CleanReason.None);

            if (!itemsToClean.Any())
            {
                this.logger.LogInformation("Nothing to clean.");
                return;
            }

            // Log the directories to be cleaned.
            foreach (IGrouping<CleanReason, ProgramDirectoryItem> group in itemsToClean.GroupBy(x => x.Reason))
            {
                this.logger.LogTrace($"Found {group.Count()} {group.Key} items to clean:");
                foreach (ProgramDirectoryItem item in group)
                {
                    this.logger.LogTrace($"\t{item.Path}");
                }
            }

            this.CleanItems(itemsToClean);
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
                throw new systemIO.InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
            }

            // Delete the empty folder.
            this.DeleteDirectory(itemToClean.Path);
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
                throw new systemIO.InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
            }

            string programRootDir = this.fileSystem.Path.GetDirectoryName(itemToClean.Path);

            IEnumerable<FileClassificationItem> files = this.fileSystem.Directory.EnumerateFiles(itemToClean.Path)
                .Select(x => new FileClassificationItem(x, this.fileClassifier.ClassifyFile(x)));

            // Move the app items to the program root directory.
            IEnumerable<string> appFilePaths = files.Where(x => x.Classification == FileClassification.App).Select(x => x.Path);
            this.MoveFilesToDirectory(programRootDir, appFilePaths, replaceExisting: true);

            // Delete the rest of the files.
            IEnumerable<string> otherFilePaths = files.Where(x => x.Classification != FileClassification.App).Select(x => x.Path);
            this.DeleteFiles(otherFilePaths);

            // Delete the empty folder.
            this.DeleteDirectory(itemToClean.Path);
        }

        private void CleanItem(ProgramDirectoryItem itemToClean)
        {
            Action<ProgramDirectoryItem> cleanFunction = this.GetCleanFunction(itemToClean.Reason);

            try
            {
                cleanFunction(itemToClean);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.logger.LogError(ex, $"Failed to clean {itemToClean.Path}. Aborting.");
            }
        }

        private void CleanItems(IEnumerable<ProgramDirectoryItem> itemsToClean)
        {
            this.logger.LogInformation("Cleaning.");

            foreach (ProgramDirectoryItem item in itemsToClean)
            {
                this.logger.LogInformation($"Cleaning {item.Reason} {item.Path}");
                this.CleanItem(item);
            }

            this.logger.LogInformation("Finished cleaning.");
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
            this.MoveFileToDirectory(programRootDir, currentFileLocation, replaceExisting: true);

            // Delete the empty folder.
            this.DeleteDirectory(itemToClean.Path);
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

        #region IO Operation Wrappers

        private void DeleteDirectory(string directoryPath)
        {
            if (!this.options.Simulate)
            {
                this.fileSystem.Directory.Delete(directoryPath);
            }

            this.logger.LogDebug($"Deleted directory: \"{this.fileSystem.Path.GetFileName(directoryPath)}\"");
        }

        private void DeleteFile(string filePath)
        {
            if (!this.fileSystem.File.Exists(filePath))
            {
                return;
            }

            if (!this.options.Simulate)
            {
                this.fileSystem.File.Delete(filePath);
            }

            this.logger.LogDebug($"Deleted file: \"{this.fileSystem.Path.GetFileName(filePath)}\"");
        }

        private void DeleteFiles(IEnumerable<string> filePaths)
        {
            foreach (string filePath in filePaths)
            {
                this.DeleteFile(filePath);
            }
        }

        private void MoveFilesToDirectory(string newDirectory, IEnumerable<string> currentFileLocations, bool replaceExisting = false)
        {
            foreach (string currentFileLocation in currentFileLocations)
            {
                this.MoveFileToDirectory(newDirectory, currentFileLocation, replaceExisting);
            }
        }

        private void MoveFileToDirectory(string newDirectory, string currentFileLocation, bool replaceExisting = false)
        {
            string newFileLocation = this.fileSystem.Path.Combine(newDirectory, this.fileSystem.Path.GetFileName(currentFileLocation));
            if (!this.options.Simulate)
            {
                if (replaceExisting && this.fileSystem.File.Exists(newFileLocation))
                {
                    this.DeleteFile(newFileLocation);
                }

                this.fileSystem.File.Move(currentFileLocation, newFileLocation);
            }

            this.logger.LogDebug($"Moved file: \"{this.fileSystem.Path.GetFileName(currentFileLocation)}\" to \"{newFileLocation}\"");
        }

        #endregion IO Operation Wrappers
    }
}
