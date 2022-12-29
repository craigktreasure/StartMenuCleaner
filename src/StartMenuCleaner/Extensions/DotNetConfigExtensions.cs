namespace StartMenuCleaner.Extensions;

using DotNetConfig;

internal static class DotNetConfigExtensions
{
    /// <summary>
    /// Gets all entries from any subsection.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="section">The section.</param>
    /// <param name="variable">The variable.</param>
    /// <returns><see cref="IEnumerable{ConfigEntry}"/>.</returns>
    public static IEnumerable<ConfigEntry> GetAllFromAnySubsection(this Config config, string section, string variable)
        => config.Where(c => c.Section == section && c.Variable == variable);
}
