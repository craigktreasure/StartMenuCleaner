namespace StartMenuCleaner
{
    using StartMenuCleaner.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Linq;

    public class FileClassifier
    {
        private const string apprefmsFileExtension = ".appref-ms";

        private const string chmFileExtension = ".chm";

        private const string exeFileExtension = ".exe";

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

        private static readonly ISet<string> uninstallerExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            exeFileExtension,
            msiFileExtension
        };

        private readonly IFileSystem fileSystem;

        private readonly IFileShortcutHandler shortcutHandler;

        public FileClassifier(IFileSystem fileSystem, IFileShortcutHandler shortcutHandler)
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
                return this.IsDeletableFile(shortcut.Target);
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
                return this.IsWebLink(shortcut.Target);
            }

            return false;
        }

        private bool IsWebLink(string filePath)
        {
            string ext = this.fileSystem.Path.GetExtension(filePath);

            return ext == urlFileExtension;
        }
    }
}
