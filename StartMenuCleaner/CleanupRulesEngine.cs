namespace StartMenuCleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using StartMenuCleaner.Utils;

    public class CleanupRulesEngine
	{
		private const string apprefmsFileExtension = ".appref-ms";
		private const string chmFileExtension = ".chm";
		private const string exeFileExtension = ".exe";
		private const string lnkFileExtension = ".lnk";
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

		public static FileClassification ClassifyFile(string filePath)
		{
			if (IsLinkToUninstaller(filePath))
			{
				return FileClassification.Uninstaller;
			}

			if (IsLinkToApp(filePath) || IsClickOnceApp(filePath))
			{
				return FileClassification.App;
			}

			if (IsWebLink(filePath) || IsLinkToWeb(filePath))
			{
				return FileClassification.WebLink;
			}

			if (IsLinkToHelp(filePath))
			{
				return FileClassification.Help;
			}

			if (IsDeletableFile(filePath) || IsLinkToDeletableFile(filePath))
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
			if (ShouldIgnoreDirectory(directoryPath))
			{
				return CleanReason.None;
			}

			if (this.TestForEmpty(directoryPath))
			{
				return CleanReason.Empty;
			}

			if (TestForDirectories(directoryPath))
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

		private static bool FileIsLink(string filePath)
		{
			string ext = Path.GetExtension(filePath);

			return ext == lnkFileExtension;
		}

		private static bool IsClickOnceApp(string filePath)
		{
			string ext = Path.GetExtension(filePath);

			return ext == apprefmsFileExtension;
		}

		private static bool IsDeletableFile(string filePath)
		{
			string ext = Path.GetExtension(filePath);

			return deletableExtensions.Contains(ext);
		}

		private static bool IsLinkToApp(string filePath)
		{
			if (FileIsLink(filePath))
			{
				string linkPath = NativeMethods.ResolveShortcut(filePath);

				string linkExt = Path.GetExtension(linkPath);

				return appExtensions.Contains(linkExt);
			}

			return false;
		}

		private static bool IsLinkToDeletableFile(string filePath)
		{
			if (FileIsLink(filePath))
			{
				string linkPath = NativeMethods.ResolveShortcut(filePath);

				string linkExt = Path.GetExtension(linkPath);

				return deletableExtensions.Contains(linkExt);
			}

			return false;
		}

		private static bool IsLinkToHelp(string filePath)
		{
			if (FileIsLink(filePath))
			{
				string linkPath = NativeMethods.ResolveShortcut(filePath);

				string linkExt = Path.GetExtension(linkPath);

				return linkExt == chmFileExtension;
			}

			return false;
		}

		private static bool IsLinkToUninstaller(string filePath)
		{
			if (!FileIsLink(filePath))
			{
				return false;
			}

			string linkPath = NativeMethods.ResolveShortcut(filePath);

			string linkExt = Path.GetExtension(linkPath);

			string fileName = Path.GetFileNameWithoutExtension(filePath);

			return uninstallerExtensions.Contains(linkExt) &&
				fileName.Contains(uninstallFileNameKeyword, StringComparison.CurrentCultureIgnoreCase);
		}

		private static bool IsLinkToWeb(string filePath)
		{
			if (FileIsLink(filePath))
			{
				string linkPath = NativeMethods.ResolveShortcut(filePath);

				string linkExt = Path.GetExtension(linkPath);

				return linkExt == urlFileExtension;
			}

			return false;
		}

		private static bool IsWebLink(string filePath)
		{
			string ext = Path.GetExtension(filePath);

			return ext == urlFileExtension;
		}

		private static bool ShouldIgnoreDirectory(string directoryPath)
		{
			string directoryName = Path.GetFileName(directoryPath);

			return directoriesToIgnore.Contains(directoryName, StringComparer.CurrentCultureIgnoreCase);
		}

		private static bool TestForDirectories(string directoryPath)
		{
			IEnumerable<string> directories = Directory.EnumerateDirectories(directoryPath);

			return directories.Any();
		}

		private bool TestForEmpty(string directoryPath)
		{
			return !Directory.EnumerateFileSystemEntries(directoryPath, "*", SearchOption.AllDirectories).Any();
		}

		private bool TestForFewAppsWithCruft(string directoryPath)
		{
			if (TestForDirectories(directoryPath))
			{
				return false;
			}

			IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath);

            // Classify the files
            IEnumerable<FileClassificationItem> classifiedFiles = filePaths.Select(x => new FileClassificationItem(x, ClassifyFile(x)));

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
			if (TestForDirectories(directoryPath))
			{
				return false;
			}

			IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath);

			if (filePaths.Count() != 1)
			{
				return false;
			}

			string filePath = filePaths.First();

			return ClassifyFile(filePath) == FileClassification.App;
		}
	}
}