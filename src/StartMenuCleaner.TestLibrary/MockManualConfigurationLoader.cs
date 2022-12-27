namespace StartMenuCleaner.TestLibrary;

using StartMenuCleaner.Utils;

/// <summary>
/// Class MockManualConfigurationLoader.
/// Implements the <see cref="IManualConfigurationLoader" />
/// </summary>
/// <seealso cref="IManualConfigurationLoader" />
public class MockManualConfigurationLoader : IManualConfigurationLoader
{
    private readonly IDictionary<string, ManualDirectoryRemoveConfiguration> directoryConfigurations;

    private readonly IDictionary<string, ManualFileRemoveConfiguration> fileConfigurations;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockManualConfigurationLoader"/> class.
    /// </summary>
    /// <param name="files">The files.</param>
    /// <param name="directories">The directories.</param>
    public MockManualConfigurationLoader(IEnumerable<string>? files = null, IDictionary<string, IEnumerable<string>?>? directories = null)
    {
        this.fileConfigurations = files?
            .ToDictionary(
                f => f,
                f => new ManualFileRemoveConfiguration(f),
                StringComparer.OrdinalIgnoreCase)
            ?? new();

        this.directoryConfigurations = directories?
            .ToDictionary(
                d => d.Key,
                d => new ManualDirectoryRemoveConfiguration(d.Key, d.Value ?? Array.Empty<string>()),
                StringComparer.OrdinalIgnoreCase)
            ?? new();
    }

    /// <summary>
    /// Loads the configurations for directory removal.
    /// </summary>
    /// <returns><see cref="IDictionary{String, ManualDirectoryRemoveConfiguration}" />.</returns>
    IDictionary<string, ManualDirectoryRemoveConfiguration> IManualConfigurationLoader.LoadDirectoryConfigurations() => this.directoryConfigurations;

    /// <summary>
    /// Loads the configurations for file removal.
    /// </summary>
    /// <returns><see cref="IDictionary{String, ManualFileRemoveConfiguration}" />.</returns>
    IDictionary<string, ManualFileRemoveConfiguration> IManualConfigurationLoader.LoadFileConfigurations() => this.fileConfigurations;
}
