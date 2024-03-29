﻿namespace StartMenuCleaner;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

using StartMenuCleaner.Utils;

internal class FileClassifier
{
    private const string apprefmsFileExtension = ".appref-ms";

    private const string chmFileExtension = ".chm";

    private const string exeFileExtension = ".exe";

    private const string msiFileExtension = ".msi";

    private const string txtFileExtension = ".txt";

    private const string uninstallFileNameKeyword = "uninstall";

    private const string urlFileExtension = ".url";

    private static readonly string[] appExtensions =
    [
        exeFileExtension,
        apprefmsFileExtension
    ];

    private static readonly string[] deletableExtensions =
    [
        txtFileExtension
    ];

    private static readonly HashSet<string> uninstallerExtensions = new(StringComparer.InvariantCultureIgnoreCase)
    {
        exeFileExtension,
        msiFileExtension
    };

    private readonly IFileSystem fileSystem;

    private readonly FileShortcutHandler shortcutHandler;

    public FileClassifier(IFileSystem fileSystem, FileShortcutHandler shortcutHandler)
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
        if (this.shortcutHandler.TryGetShortcut(filePath, out FileShortcut? shortcut))
        {
            string linkExt = this.fileSystem.Path.GetExtension(shortcut.TargetPath);

            return appExtensions.Contains(linkExt);
        }

        return false;
    }

    private bool IsLinkToDeletableFile(string filePath)
    {
        if (this.shortcutHandler.TryGetShortcut(filePath, out FileShortcut? shortcut))
        {
            return this.IsDeletableFile(shortcut.TargetPath);
        }

        return false;
    }

    private bool IsLinkToHelp(string filePath)
    {
        if (this.shortcutHandler.TryGetShortcut(filePath, out FileShortcut? shortcut))
        {
            string linkExt = this.fileSystem.Path.GetExtension(shortcut.TargetPath);

            return linkExt == chmFileExtension;
        }

        return false;
    }

    private bool IsLinkToUninstaller(string filePath)
    {
        if (!this.shortcutHandler.TryGetShortcut(filePath, out FileShortcut? shortcut))
        {
            return false;
        }

        string linkExt = this.fileSystem.Path.GetExtension(shortcut.TargetPath);

        string fileName = this.fileSystem.Path.GetFileNameWithoutExtension(filePath);

        return uninstallerExtensions.Contains(linkExt) &&
            fileName.Contains(uninstallFileNameKeyword, StringComparison.CurrentCultureIgnoreCase);
    }

    private bool IsLinkToWeb(string filePath)
    {
        if (this.shortcutHandler.TryGetShortcut(filePath, out FileShortcut? shortcut))
        {
            return this.IsWebLink(shortcut.TargetPath);
        }

        return false;
    }

    private bool IsWebLink(string filePath)
    {
        string ext = this.fileSystem.Path.GetExtension(filePath);

        return ext == urlFileExtension;
    }
}
