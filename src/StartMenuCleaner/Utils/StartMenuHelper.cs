﻿namespace StartMenuCleaner.Utils;

using IPath = Path;

internal static class StartMenuHelper
{
    private const string programsFolderName = "Programs";

    /// <summary>
    /// Gets the known start menu folders.
    /// </summary>
    /// <returns>An <see cref="IReadOnlyList{T}"/> of <see cref="string"/>.</returns>
    public static IReadOnlyList<string> GetKnownStartMenuFolders() => [
        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
    ];

    /// <summary>
    /// Gets the known start menu programs folders.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="string"/>.</returns>
    public static IReadOnlyList<string> GetKnownStartMenuProgramsFolders() =>
        [.. GetKnownStartMenuFolders().Select(x => IPath.Combine(x, programsFolderName))];
}
