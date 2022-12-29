namespace StartMenuCleaner.Cleaners;

internal interface ICleaner
{
    /// <summary>
    /// Gets the cleaner type.
    /// </summary>
    CleanerType CleanerType { get; }

    /// <summary>
    /// Determines if the cleaner can clean the specified item path.
    /// </summary>
    /// <param name="itemPath">The item path.</param>
    /// <returns><c>true</c> if the cleaner can clean the item path, <c>false</c> otherwise.</returns>
    bool CanClean(string itemPath);

    /// <summary>
    /// Cleans the specified item path.
    /// </summary>
    /// <param name="itemPath">The item path.</param>
    void Clean(string itemPath);
}
