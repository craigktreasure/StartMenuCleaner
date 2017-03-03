using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using StartMenuCleaner.Utils;

namespace StartMenuCleaner
{
	public class Cleaner
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private readonly bool simulate;

		public Cleaner(bool simulate)
		{
			this.simulate = simulate;
		}

		public void Start()
		{
			if (this.simulate)
			{
				logger.Info("Simulating. No changes will be made.");
				Console.WriteLine();
			}

			IEnumerable<string> programDirectories = this.GetProgramDirectories();

			CleanupRulesEngine rules = new CleanupRulesEngine();
			IEnumerable<ProgramDirectoryItem> itemsToClean = programDirectories
				.Select(x => new ProgramDirectoryItem(x, rules.TestForCleanReason(x)))
				.Where(x => x.Reason != CleanReason.None);

			if (!itemsToClean.Any())
			{
				logger.Info("Nothing to clean.");
				return;
			}

			// Log the directories to be cleaned.
			var itemGroups = itemsToClean.GroupBy(x => x.Reason);
			foreach (var group in itemGroups)
			{
				Console.WriteLine($"Found {group.Count()} {group.Key} items to clean:");
				foreach (ProgramDirectoryItem item in group)
				{
					Console.WriteLine($"\t{item.Path}");
				}
			}

			this.CleanItems(rules, itemsToClean);
		}

		private void CleanEmptyDirectory(CleanupRulesEngine rules, ProgramDirectoryItem itemToClean)
		{
			if (CleanReason.Empty != itemToClean.Reason)
			{
				throw new ArgumentException($"The item is not a {CleanReason.Empty}.", nameof(itemToClean.Reason));
			}

			var testFunction = rules.GetReasonTestFunction(CleanReason.Empty);
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
				throw new ArgumentException($"The item is not a {CleanReason.FewAppsWithCruft}.", nameof(itemToClean.Reason));
			}

			var testFunction = rules.GetReasonTestFunction(CleanReason.FewAppsWithCruft);
			if (!testFunction(itemToClean.Path))
			{
				throw new InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
			}

			string programRootDir = Path.GetDirectoryName(itemToClean.Path);

			IEnumerable<FileClassificationItem> files = Directory.EnumerateFiles(itemToClean.Path)
				.Select(x => new FileClassificationItem(x, rules.ClassifyFile(x)));

			// Move the app items to the program root directory.
			var appFilePaths = files.Where(x => x.Classification == FileClassification.App).Select(x => x.Path);
			this.MoveFilesToDirectory(programRootDir, appFilePaths, replaceExisting: true);

			// Delete the rest of the files.
			var otherFilePaths = files.Where(x => x.Classification != FileClassification.App).Select(x => x.Path);
			this.DeleteFiles(otherFilePaths);

			// Delete the empty folder.
			this.DeleteDirectory(itemToClean.Path);
		}

		private void CleanItem(CleanupRulesEngine rules, ProgramDirectoryItem itemToClean)
		{
			var cleanFunction = this.GetCleanFunction(itemToClean.Reason);

			try
			{
				cleanFunction(rules, itemToClean);
			}
			catch (Exception ex)
			{
				logger.Error(ex, $"Failed to clean {itemToClean.Path}. Aborting.");
			}
		}

		private void CleanItems(CleanupRulesEngine rules, IEnumerable<ProgramDirectoryItem> itemsToClean)
		{
			logger.Debug("Cleaning.");

			foreach (var item in itemsToClean)
			{
				logger.Debug($"Cleaning {item.Reason} {item.Path}");
				this.CleanItem(rules, item);
			}

			logger.Debug("Finished cleaning.");
		}

		private void CleanSingleApp(CleanupRulesEngine rules, ProgramDirectoryItem itemToClean)
		{
			if (CleanReason.SingleApp != itemToClean.Reason)
			{
				throw new ArgumentException($"The item is not a {CleanReason.SingleApp}.", nameof(itemToClean.Reason));
			}

			var testFunction = rules.GetReasonTestFunction(CleanReason.SingleApp);
			if (!testFunction(itemToClean.Path))
			{
				throw new InvalidDataException($"The path is not a valid {itemToClean.Reason} folder.");
			}

			string programRootDir = Path.GetDirectoryName(itemToClean.Path);

			// Move the only file into the program root directory.
			string currentFileLocation = Directory.GetFiles(itemToClean.Path).First();
			this.MoveFileToDirectory(programRootDir, currentFileLocation, replaceExisting: true);

			// Delete the empty folder.
			this.DeleteDirectory(itemToClean.Path);
		}

		private Action<CleanupRulesEngine, ProgramDirectoryItem> GetCleanFunction(CleanReason reason)
		{
			switch (reason)
			{
				case CleanReason.Empty:
					return this.CleanEmptyDirectory;

				case CleanReason.SingleApp:
					return this.CleanSingleApp;

				case CleanReason.FewAppsWithCruft:
					return this.CleanFewAppsWithCruft;
			}

			throw new ArgumentException("No cleanup function was available.", nameof(reason));
		}

		private IEnumerable<string> GetProgramDirectories()
		{
			IEnumerable<string> startMenuProgramDirectories = StartMenuHelper.GetStartMenuProgramDirectories();

			IEnumerable<string> programDirectories = startMenuProgramDirectories.SelectMany(x => Directory.GetDirectories(x));

			return programDirectories;
		}

		#region IO Operation Wrappers

		private void DeleteDirectory(string directoryPath)
		{
			if (!this.simulate)
			{
				Directory.Delete(directoryPath);
			}
			logger.Debug($"Deleted directory: \"{Path.GetFileName(directoryPath)}\"");
		}

		private void DeleteFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return;
			}

			if (!this.simulate)
			{
				File.Delete(filePath);
			}

			logger.Debug($"Deleted file: \"{Path.GetFileName(filePath)}\"");
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
			if (!this.simulate)
			{
				if (replaceExisting && File.Exists(newFileLocation))
				{
					this.DeleteFile(newFileLocation);
				}

				File.Move(currentFileLocation, newFileLocation);
			}
			logger.Debug($"Moved file: \"{Path.GetFileName(currentFileLocation)}\" to \"{newFileLocation}\"");
		}

		#endregion IO Operation Wrappers
	}
}