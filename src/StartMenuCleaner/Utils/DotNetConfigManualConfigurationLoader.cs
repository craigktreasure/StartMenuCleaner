namespace StartMenuCleaner.Utils;

using DotNetConfig;

using StartMenuCleaner.Extensions;

internal class DotNetConfigManualConfigurationLoader : IManualConfigurationLoader
{
    private readonly Config configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetConfigManualConfigurationLoader" /> class.
    /// </summary>
    /// <param name="configurationPath">The configuration path.</param>
    public DotNetConfigManualConfigurationLoader(string? configurationPath = null)
    {
        this.configuration = Config.Build(configurationPath);
    }

    /// <summary>
    /// Loads the configurations for directory removal.
    /// </summary>
    /// <returns><see cref="IDictionary{String, ManualCleanConfiguration}" />.</returns>
    public IDictionary<string, ManualDirectoryRemoveConfiguration> LoadDirectoryConfigurations()
        => LoadDirectoryItems(this.configuration)
            .ToDictionary(x => x.DirectoryName, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Loads the configurations for file removal.
    /// </summary>
    /// <returns><see cref="IDictionary{String, ManualFileRemoveConfiguration}" />.</returns>
    public IDictionary<string, ManualFileRemoveConfiguration> LoadFileConfigurations()
        => LoadFileItems(this.configuration)
            .ToDictionary(x => x.FileName, StringComparer.OrdinalIgnoreCase);

    private static ManualDirectoryRemoveConfiguration? LoadDirectoryItem(Config config, ConfigEntry entry)
    {
        string? directoryName = entry.Subsection
            ?? throw new InvalidOperationException("The subsection cannot be null.");

        IEnumerable<string> filesToPromote = config
            .GetAll(Constants.ConfigurationSectionName, directoryName, "promote")
            .Select(x => x.GetString());

        return new ManualDirectoryRemoveConfiguration(directoryName.Trim('/'), filesToPromote);
    }

    private static IEnumerable<ManualDirectoryRemoveConfiguration> LoadDirectoryItems(Config config)
    {
        IEnumerable<ConfigEntry> configuredDirectories = config
            .GetAllFromAnySubsection(Constants.ConfigurationSectionName, "remove")
            .Where(c => (c.Subsection?.EndsWith('/') ?? false) && c.GetBoolean());

        foreach (ConfigEntry entry in configuredDirectories)
        {
            ManualDirectoryRemoveConfiguration? configuration = LoadDirectoryItem(config, entry);

            if (configuration is not null)
            {
                yield return configuration;
            }
        }
    }

    private static IEnumerable<ManualFileRemoveConfiguration> LoadFileItems(Config config)
    {
        IEnumerable<ConfigEntry> configuredFiles = config
            .GetAllFromAnySubsection(Constants.ConfigurationSectionName, "remove")
            .Where(c => c.Subsection is not null && !c.Subsection.EndsWith('/') && c.GetBoolean());

        foreach (ConfigEntry entry in configuredFiles)
        {
            string? fileName = entry.Subsection
                ?? throw new InvalidOperationException("The subsection cannot be null.");

            yield return new ManualFileRemoveConfiguration(fileName);
        }
    }
}
