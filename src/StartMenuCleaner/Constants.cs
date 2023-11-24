namespace StartMenuCleaner;

internal static class Constants
{
    public const string ConfigurationSectionName = "startmenucleaner";

    public static readonly string[] DirectoriesToIgnore =
    [
        "chrome apps",
        "startup",
        "maintenance",
        "accessories",
        "windows accessories",
        "windows administrative tools",
        "windows ease of access",
        "windows powershell",
        "windows system",
        "accessibility",
        "administrative tools",
        "system tools"
    ];
}
