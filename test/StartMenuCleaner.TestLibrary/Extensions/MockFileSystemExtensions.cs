namespace StartMenuCleaner.TestLibrary.Extensions;

using System;
using System.IO.Abstractions.TestingHelpers;

/// <summary>
/// Class MockFileSystemExtensions.
/// </summary>
public static class MockFileSystemExtensions
{
    /// <summary>
    /// Adds an empty file to the mock file system.
    /// </summary>
    /// <param name="mockFileSystem">The mock file system.</param>
    /// <param name="path">The path.</param>
    public static void AddEmptyFile(this MockFileSystem mockFileSystem, string path)
    {
        ArgumentNullException.ThrowIfNull(mockFileSystem);

        mockFileSystem.AddFile(path, new MockFileData([]));
    }
}
