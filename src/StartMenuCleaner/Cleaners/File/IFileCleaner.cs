namespace StartMenuCleaner.Cleaners.File;

internal interface IFileCleaner
{
    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    FileCleanerType CleanerType { get; }

    /// <summary>
    /// Determines if the cleaner can clean the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns><c>true</c> if the cleaner can clean the file path, <c>false</c> otherwise.</returns>
    bool CanClean(string filePath);

    /// <summary>
    /// Cleans the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    void Clean(string filePath);
}
