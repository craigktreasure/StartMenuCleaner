namespace StartMenuCleaner.Utils;

using System;
using System.Diagnostics.CodeAnalysis;

internal interface IFileShortcutHandler
{
    private const string lnkFileExtension = ".lnk";

    /// <summary>
    /// Get a file shortcut.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="FileShortcut"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public FileShortcut GetShortcut(string filePath)
    {
        if (this.TryGetShortcut(filePath, out FileShortcut? result))
        {
            return result;
        }

        throw new ArgumentException("The path specified is not a valid file shortcut.", nameof(filePath));
    }

    /// <summary>
    /// Determines whether the specified file path is a shortcut.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns><see cref="bool"/>.</returns>
    public bool IsShortcut(string filePath)
    {
        string ext = Path.GetExtension(filePath);

        return ext == lnkFileExtension;
    }

    /// <summary>
    /// Resolves the target path of the specified shortcut.
    /// </summary>
    /// <param name="shortcutPath">The shortcut path.</param>
    /// <returns><see cref="string"/>.</returns>
    string ResolveTarget(string shortcutPath);

    /// <summary>
    /// Tries to get a file shortcut.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="shortcut">The shortcut.</param>
    /// <returns><see cref="bool"/>.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public bool TryGetShortcut(string filePath, [NotNullWhen(true)] out FileShortcut? shortcut)
    {
        shortcut = null;

        if (!this.IsShortcut(filePath))
        {
            return false;
        }

        try
        {
            string target = this.ResolveTarget(filePath);

            shortcut = new FileShortcut(filePath, target);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
