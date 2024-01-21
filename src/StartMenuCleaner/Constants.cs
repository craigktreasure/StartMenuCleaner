namespace StartMenuCleaner;

internal static class Constants
{
    public const string ConfigurationSectionName = "startmenucleaner";

    public static readonly string[] DirectoriesToIgnore =
    [
        "accessibility",
        "accessories",
        "administrative tools",
        "chrome apps",
        "maintenance",
        "startup",
        "system tools",
        "windows accessories",
        "windows administrative tools",
        "windows ease of access",
        "windows powershell",
        "windows system",
    ];
}
