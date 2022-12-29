namespace StartMenuCleaner.Utils;

using System.Collections.Generic;

internal interface IManualConfigurationLoader
{
    /// <summary>
    /// Loads the configurations for directory removal.
    /// </summary>
    /// <returns><see cref="IDictionary{String, ManualCleanConfiguration}"/>.</returns>
    IDictionary<string, ManualDirectoryRemoveConfiguration> LoadDirectoryConfigurations();

    /// <summary>
    /// Loads the configurations for file removal.
    /// </summary>
    /// <returns><see cref="IDictionary{String, ManualFileRemoveConfiguration}"/>.</returns>
    IDictionary<string, ManualFileRemoveConfiguration> LoadFileConfigurations();
}
