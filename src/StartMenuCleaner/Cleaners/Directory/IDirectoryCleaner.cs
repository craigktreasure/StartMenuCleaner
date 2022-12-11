namespace StartMenuCleaner.Cleaners.Directory;

internal interface IDirectoryCleaner
{
    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    DirectoryCleanerType CleanerType { get; }

    /// <summary>
    /// Determines if the cleaner can clean the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <returns><c>true</c> if the cleaner can clean the directory path, <c>false</c> otherwise.</returns>
    bool CanClean(string directoryPath);

    /// <summary>
    /// Cleans the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    void Clean(string directoryPath);
}
