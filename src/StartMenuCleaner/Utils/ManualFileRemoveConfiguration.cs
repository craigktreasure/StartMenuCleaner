namespace StartMenuCleaner.Utils;

internal sealed class ManualFileRemoveConfiguration
{
    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualFileRemoveConfiguration"/> class.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    public ManualFileRemoveConfiguration(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        this.FileName = fileName;
    }
}
