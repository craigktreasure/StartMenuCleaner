namespace StartMenuCleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;

    internal class CleanupRulesEngine
    {
        private const int minCruftApps = 2;

        private static readonly string[] directoriesToIgnore = new string[]
        {
            "chrome apps",
            "startup",
            "maintenance",
            "accessories",
            "windows accessories",
            "windows administrative tools",
            "windows ease of access",
            "windows powershell",
            "windows system",
            "accessibility",
            "administrative tools",
            "system tools"
        };

        private readonly FileClassifier fileClassifier;

        private readonly IFileSystem fileSystem;

        public CleanupRulesEngine(IFileSystem fileSystem, FileClassifier fileClassifier)
        {
            this.fileSystem = fileSystem;
            this.fileClassifier = fileClassifier;
        }

        public Func<string, bool> GetReasonTestFunction(CleanReason reason)
        {
            return reason switch
            {
                CleanReason.Empty => this.TestForEmpty,
                CleanReason.FewAppsWithCruft => this.TestForFewAppsWithCruft,
                CleanReason.SingleApp => this.TestForSingleApp,
                CleanReason.None => x => this.TestForCleanReason(x) == CleanReason.None,
                _ => throw new ArgumentException("An invalid reason was encountered.", nameof(reason)),
            };
        }

        public CleanReason TestForCleanReason(string directoryPath)
        {
            if (this.ShouldIgnoreDirectory(directoryPath))
            {
                return CleanReason.None;
            }

            if (this.TestForEmpty(directoryPath))
            {
                return CleanReason.Empty;
            }

            if (this.TestForSingleApp(directoryPath))
            {
                return CleanReason.SingleApp;
            }

            if (this.TestForFewAppsWithCruft(directoryPath))
            {
                return CleanReason.FewAppsWithCruft;
            }

            return CleanReason.None;
        }

        private static bool CanBeRemoved(FileClassification classification)
            => classification is not FileClassification.App and not FileClassification.Other;

        private bool ShouldIgnoreDirectory(string directoryPath)
        {
            string directoryName = this.fileSystem.Path.GetFileName(directoryPath);

            return directoriesToIgnore.Contains(directoryName, StringComparer.CurrentCultureIgnoreCase);
        }

        private bool TestForDirectories(string directoryPath)
        {
            IEnumerable<string> directories = this.fileSystem.Directory.EnumerateDirectories(directoryPath);

            return directories.Any();
        }

        private bool TestForEmpty(string directoryPath)
        {
            return !this.fileSystem.Directory.EnumerateFileSystemEntries(
                directoryPath, "*", System.IO.SearchOption.AllDirectories).Any();
        }

        private bool TestForFewAppsWithCruft(string directoryPath)
        {
            if (this.TestForDirectories(directoryPath))
            {
                return false;
            }

            IEnumerable<string> filePaths = this.fileSystem.Directory.EnumerateFiles(directoryPath);

            // Classify the files
            IEnumerable<FileClassificationItem> classifiedFiles = filePaths.Select(x =>
                new FileClassificationItem(x, this.fileClassifier.ClassifyFile(x)));

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

        private bool TestForSingleApp(string directoryPath)
        {
            if (this.TestForDirectories(directoryPath))
            {
                return false;
            }

            IEnumerable<string> filePaths = this.fileSystem.Directory.EnumerateFiles(directoryPath);

            if (filePaths.Count() != 1)
            {
                return false;
            }

            string filePath = filePaths.First();

            return this.fileClassifier.ClassifyFile(filePath) == FileClassification.App;
        }
    }
}
