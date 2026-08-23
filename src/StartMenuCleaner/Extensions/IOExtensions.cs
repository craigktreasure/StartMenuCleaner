namespace StartMenuCleaner.Extensions;

using System.IO.Abstractions;

internal static class IOExtensions
{
    /// <summary>
    /// Determines whether the specified directory path is empty.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><c>true</c> if the specified directory path is empty; otherwise, <c>false</c>.</returns>
    public static bool IsEmpty(this IDirectory directory, string directoryPath)
        => !directory.EnumerateFileSystemEntries(directoryPath).Any();
}
