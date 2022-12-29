namespace StartMenuCleaner.TestLibrary;

using StartMenuCleaner.TestLibrary.Extensions;
using StartMenuCleaner.Utils;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

/// <summary>
/// Class MockFileSystemComposer.
/// </summary>
public class MockFileSystemComposer
{
    private readonly IReadOnlyList<string> defaultFileSystemNodes;

    /// <summary>
    /// Gets the file system.
    /// </summary>
    public MockFileSystem FileSystem { get; } = new MockFileSystem();

    /// <summary>
    /// Gets the shortcut handler.
    /// </summary>
    public TestFileShortcutHandler ShortcutHandler { get; } = new TestFileShortcutHandler();

    /// <summary>
    /// Initializes a new instance of the <see cref="MockFileSystemComposer"/> class.
    /// </summary>
    public MockFileSystemComposer()
    {
        this.defaultFileSystemNodes = this.FileSystem.AllNodes.ToArray();
    }

    /// <summary>
    /// Adds the specified file shortcut to the file system.
    /// </summary>
    /// <param name="fileShortcut">The file shortcut.</param>
    public void Add(FileShortcut fileShortcut)
    {
        ArgumentNullException.ThrowIfNull(fileShortcut);

        this.AddFile(fileShortcut.FilePath);
        this.ShortcutHandler.AddShortcutMapping(fileShortcut);
    }

    /// <summary>
    /// Adds the specified files and folders to the file system.
    /// </summary>
    /// <param name="filesAndFolders">The files and folders.</param>
    public void Add(params string[] filesAndFolders)
    {
        ArgumentNullException.ThrowIfNull(filesAndFolders);

        foreach (string path in filesAndFolders)
        {
            if (FileShortcut.TryConvertFrom(path, out FileShortcut? fileShortcut))
            {
                this.Add(fileShortcut);
            }
            else if (this.FileSystem.Path.HasExtension(path))
            {
                this.AddFile(path);
            }
            else
            {
                this.AddDirectory(path);
            }
        }
    }

    /// <summary>
    /// Add a directory to the file system.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    public void AddDirectory(string directoryPath) =>
        this.FileSystem.AddDirectory(directoryPath);

    /// <summary>
    /// Add a file to the file system.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public void AddFile(string filePath)
    {
        if (FileShortcut.TryConvertFrom(filePath, out FileShortcut? fileShortcut))
        {
            this.Add(fileShortcut);
        }
        else
        {
            this.FileSystem.AddEmptyFile(filePath);
        }
    }

    /// <summary>
    /// Gets all nodes (excluding default nodes).
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="string"/>.</returns>
    public IEnumerable<string> GetAllNodes() =>
        this.FileSystem.AllNodes.Except(this.defaultFileSystemNodes);
}
