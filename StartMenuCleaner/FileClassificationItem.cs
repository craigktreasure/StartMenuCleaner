namespace StartMenuCleaner
{
	public class FileClassificationItem
	{
		public FileClassification Classification { get; private set; }
		public string Path { get; private set; }

		public FileClassificationItem(string path, FileClassification classification)
		{
			this.Path = path;
			this.Classification = classification;
		}
	}
}