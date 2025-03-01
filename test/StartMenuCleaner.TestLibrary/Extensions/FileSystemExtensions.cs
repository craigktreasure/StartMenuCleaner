namespace StartMenuCleaner.TestLibrary.Extensions;

using System.IO.Abstractions;

/// <summary>
/// Extensions for the <see cref="IFileSystem"/> interface.
/// </summary>
public static class FileSystemExtensions
{
    /// <summary>
    /// Gets a temporary file path.
    /// </summary>
    /// <param name="path">The file system path object.</param>
    /// <returns>A temporary file path.</returns>
    public static string GetTemporaryFilePath(this IPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        string temporaryFilePath = path.Combine(path.GetTempPath(), path.GetRandomFileName());

        path.FileSystem.File.Create(temporaryFilePath).Dispose();

        return temporaryFilePath;
    }
}
