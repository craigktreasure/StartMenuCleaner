namespace StartMenuCleaner;

internal class FileClassificationItem
{
    public FileClassification Classification { get; }

    public string Path { get; }

    public FileClassificationItem(string path, FileClassification classification)
    {
        this.Path = path;
        this.Classification = classification;
    }
}
