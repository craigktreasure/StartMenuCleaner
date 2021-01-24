namespace StartMenuCleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;
    using StartMenuCleaner.Utils;

    public class CleanupRulesEngine
	{
		private const string apprefmsFileExtension = ".appref-ms";
		private const string chmFileExtension = ".chm";
		private const string exeFileExtension = ".exe";
		private const int minCruftApps = 2;
		private const string msiFileExtension = ".msi";
		private const string txtFileExtension = ".txt";
		private const string uninstallFileNameKeyword = "uninstall";
		private const string urlFileExtension = ".url";

		private static readonly string[] appExtensions = new string[]
		{
			exeFileExtension,
			apprefmsFileExtension
		};

		private static readonly string[] deletableExtensions = new string[]
		{
			txtFileExtension
		};

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

		private static readonly ISet<string> uninstallerExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			exeFileExtension,
			msiFileExtension
		};

        private readonly IFileSystem fileSystem;

        private readonly IFileShortcutHandler shortcutHandler;

        public CleanupRulesEngine(IFileSystem fileSystem, IFileShortcutHandler shortcutHandler)
        {
            this.fileSystem = fileSystem;
            this.shortcutHandler = shortcutHandler;
        }

		public FileClassification ClassifyFile(string filePath)
		{
			if (this.IsLinkToUninstaller(filePath))
			{
				return FileClassification.Uninstaller;
			}

			if (this.IsLinkToApp(filePath) || this.IsClickOnceApp(filePath))
			{
				return FileClassification.App;
			}

			if (this.IsWebLink(filePath) || this.IsLinkToWeb(filePath))
			{
				return FileClassification.WebLink;
			}

			if (this.IsLinkToHelp(filePath))
			{
				return FileClassification.Help;
			}

			if (this.IsDeletableFile(filePath) || this.IsLinkToDeletableFile(filePath))
			{
				return FileClassification.OtherDeletable;
			}

			return FileClassification.Other;
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

			if (this.TestForDirectories(directoryPath))
			{
				return CleanReason.None;
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
		{
			return classification != FileClassification.App && classification != FileClassification.Other;
		}

		private bool IsClickOnceApp(string filePath)
		{
			string ext = this.fileSystem.Path.GetExtension(filePath);

			return ext == apprefmsFileExtension;
		}

		private bool IsDeletableFile(string filePath)
		{
			string ext = this.fileSystem.Path.GetExtension(filePath);

			return deletableExtensions.Contains(ext);
		}

		private bool IsLinkToApp(string filePath)
		{
			if (this.shortcutHandler.TryCreateShortcut(filePath, out FileShortcut? shortcut))
			{
				string linkExt = this.fileSystem.Path.GetExtension(shortcut.Target);

				return appExtensions.Contains(linkExt);
			}

			return false;
		}

		private bool IsLinkToDeletableFile(string filePath)
		{
			if (this.shortcutHandler.TryCreateShortcut(filePath, out FileShortcut? shortcut))
			{
				string linkExt = this.fileSystem.Path.GetExtension(shortcut.Target);

				return deletableExtensions.Contains(linkExt);
			}

			return false;
		}

		private bool IsLinkToHelp(string filePath)
		{
			if (this.shortcutHandler.TryCreateShortcut(filePath, out FileShortcut? shortcut))
			{
				string linkExt = this.fileSystem.Path.GetExtension(shortcut.Target);

				return linkExt == chmFileExtension;
			}

			return false;
		}

		private bool IsLinkToUninstaller(string filePath)
		{
			if (!this.shortcutHandler.TryCreateShortcut(filePath, out FileShortcut? shortcut))
			{
				return false;
			}

			string linkExt = this.fileSystem.Path.GetExtension(shortcut.Target);

			string fileName = this.fileSystem.Path.GetFileNameWithoutExtension(filePath);

			return uninstallerExtensions.Contains(linkExt) &&
				fileName.Contains(uninstallFileNameKeyword, StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsLinkToWeb(string filePath)
		{
			if (this.shortcutHandler.TryCreateShortcut(filePath, out FileShortcut? shortcut))
			{
				string linkExt = this.fileSystem.Path.GetExtension(shortcut.Target);

				return linkExt == urlFileExtension;
			}

			return false;
		}

		private bool IsWebLink(string filePath)
		{
			string ext = this.fileSystem.Path.GetExtension(filePath);

			return ext == urlFileExtension;
		}

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
            IEnumerable<FileClassificationItem> classifiedFiles = filePaths.Select(x => new FileClassificationItem(x, this.ClassifyFile(x)));

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

			return this.ClassifyFile(filePath) == FileClassification.App;
		}
	}
}