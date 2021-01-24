namespace StartMenuCleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using StartMenuCleaner.Utils;

    public class Cleaner
	{
        private readonly ILogger<Cleaner> logger;
        private readonly CleanerOptions options;

        public Cleaner(CleanerOptions options, ILogger<Cleaner> logger)
		{
            this.options = options;
            this.logger = logger;
		}

		public void Start()
		{
			if (this.options.Simulate)
			{
				this.logger.LogInformation("Simulating. No changes will be made.");
				Console.WriteLine();
			}

			IEnumerable<string> programDirectories = GetProgramDirectories();

			CleanupRulesEngine rules = new CleanupRulesEngine();
			IEnumerable<ProgramDirectoryItem> itemsToClean = programDirectories
				.Select(x => new ProgramDirectoryItem(x, rules.TestForCleanReason(x)))
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

			this.CleanItems(rules, itemsToClean);
		}

		private void CleanEmptyDirectory(CleanupRulesEngine rules, ProgramDirectoryItem itemToClean)
		{
			if (CleanReason.Empty != itemToClean.Reason)
			{
				throw new ArgumentException($"The item {nameof(itemToClean.Reason)} is not {CleanReason.Empty}.", nameof(itemToClean));
			}

			Func<string, bool> testFunction = rules.GetReasonTestFunction(CleanReason.Empty);
			if (!testFunction(itemToClean.Path))
			{
				throw new InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
			}

			// Delete the empty folder.
			this.DeleteDirectory(itemToClean.Path);
		}

		private void CleanFewAppsWithCruft(CleanupRulesEngine rules, ProgramDirectoryItem itemToClean)
		{
			if (CleanReason.FewAppsWithCruft != itemToClean.Reason)
			{
				throw new ArgumentException($"The item {nameof(itemToClean.Reason)} is not {CleanReason.FewAppsWithCruft}.", nameof(itemToClean));
			}

			Func<string, bool> testFunction = rules.GetReasonTestFunction(CleanReason.FewAppsWithCruft);
			if (!testFunction(itemToClean.Path))
			{
				throw new InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
			}

			string programRootDir = Path.GetDirectoryName(itemToClean.Path)!;

			IEnumerable<FileClassificationItem> files = Directory.EnumerateFiles(itemToClean.Path)
				.Select(x => new FileClassificationItem(x, CleanupRulesEngine.ClassifyFile(x)));

			// Move the app items to the program root directory.
			IEnumerable<string> appFilePaths = files.Where(x => x.Classification == FileClassification.App).Select(x => x.Path);
			this.MoveFilesToDirectory(programRootDir, appFilePaths, replaceExisting: true);

			// Delete the rest of the files.
			IEnumerable<string> otherFilePaths = files.Where(x => x.Classification != FileClassification.App).Select(x => x.Path);
			this.DeleteFiles(otherFilePaths);

			// Delete the empty folder.
			this.DeleteDirectory(itemToClean.Path);
		}

		private void CleanItem(CleanupRulesEngine rules, ProgramDirectoryItem itemToClean)
		{
			Action<CleanupRulesEngine, ProgramDirectoryItem> cleanFunction = this.GetCleanFunction(itemToClean.Reason);

			try
			{
				cleanFunction(rules, itemToClean);
			}
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
				this.logger.LogError(ex, $"Failed to clean {itemToClean.Path}. Aborting.");
			}
		}

		private void CleanItems(CleanupRulesEngine rules, IEnumerable<ProgramDirectoryItem> itemsToClean)
		{
			this.logger.LogInformation("Cleaning.");

			foreach (ProgramDirectoryItem item in itemsToClean)
			{
				this.logger.LogInformation($"Cleaning {item.Reason} {item.Path}");
				this.CleanItem(rules, item);
			}

			this.logger.LogInformation("Finished cleaning.");
		}

		private void CleanSingleApp(CleanupRulesEngine rules, ProgramDirectoryItem itemToClean)
		{
			if (CleanReason.SingleApp != itemToClean.Reason)
			{
				throw new ArgumentException($"The item {nameof(itemToClean.Reason)} is not {CleanReason.SingleApp}.", nameof(itemToClean));
			}

			Func<string, bool> testFunction = rules.GetReasonTestFunction(CleanReason.SingleApp);
			if (!testFunction(itemToClean.Path))
			{
				throw new InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
			}

			string programRootDir = Path.GetDirectoryName(itemToClean.Path)!;

			// Move the only file into the program root directory.
			string currentFileLocation = Directory.GetFiles(itemToClean.Path).First();
			this.MoveFileToDirectory(programRootDir, currentFileLocation, replaceExisting: true);

			// Delete the empty folder.
			this.DeleteDirectory(itemToClean.Path);
		}

		private Action<CleanupRulesEngine, ProgramDirectoryItem> GetCleanFunction(CleanReason reason)
		{
            return reason switch
            {
                CleanReason.Empty => this.CleanEmptyDirectory,
                CleanReason.SingleApp => this.CleanSingleApp,
                CleanReason.FewAppsWithCruft => this.CleanFewAppsWithCruft,
                _ => throw new ArgumentException("No cleanup function was available.", nameof(reason)),
            };
        }

		private static IEnumerable<string> GetProgramDirectories()
		{
			IEnumerable<string> startMenuProgramDirectories = StartMenuHelper.GetStartMenuProgramDirectories();

			IEnumerable<string> programDirectories = startMenuProgramDirectories.SelectMany(x => Directory.GetDirectories(x));

			return programDirectories;
		}

		#region IO Operation Wrappers

		private void DeleteDirectory(string directoryPath)
		{
			if (!this.options.Simulate)
			{
				Directory.Delete(directoryPath);
			}

			this.logger.LogDebug($"Deleted directory: \"{Path.GetFileName(directoryPath)}\"");
		}

		private void DeleteFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return;
			}

			if (!this.options.Simulate)
			{
				File.Delete(filePath);
			}

            this.logger.LogDebug($"Deleted file: \"{Path.GetFileName(filePath)}\"");
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
			string newFileLocation = Path.Combine(newDirectory, Path.GetFileName(currentFileLocation));
			if (!this.options.Simulate)
			{
				if (replaceExisting && File.Exists(newFileLocation))
				{
					this.DeleteFile(newFileLocation);
				}

				File.Move(currentFileLocation, newFileLocation);
			}

            this.logger.LogDebug($"Moved file: \"{Path.GetFileName(currentFileLocation)}\" to \"{newFileLocation}\"");
		}

		#endregion IO Operation Wrappers
	}
}