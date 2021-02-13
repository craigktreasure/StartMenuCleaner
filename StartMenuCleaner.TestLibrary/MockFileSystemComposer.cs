namespace StartMenuCleaner.TestLibrary
{
    using StartMenuCleaner.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO.Abstractions.TestingHelpers;

    public class MockFileSystemComposer
    {
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
        }

        /// <summary>
        /// Adds the specified file shortcut to the file system.
        /// </summary>
        /// <param name="fileShortcut">The file shortcut.</param>
        public void Add(FileShortcut fileShortcut)
        {
            this.AddFile(fileShortcut.FilePath);
            this.ShortcutHandler.AddShortcutMapping(fileShortcut);
        }

        /// <summary>
        /// Add a directory to the file system.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        public void AddDirectory(string directoryPath)
        {
            this.FileSystem.AddDirectory(directoryPath);
        }

        /// <summary>
        /// Add a file to the file system.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void AddFile(string filePath)
        {
            if (FileShortcutSyntax.TryConvertFrom(filePath, out FileShortcutSyntax? fileShortcut))
            {
                this.Add(fileShortcut);
            }
            else
            {
                this.FileSystem.AddFile(filePath, MockFileData.NullObject);
            }
        }
    }
}
