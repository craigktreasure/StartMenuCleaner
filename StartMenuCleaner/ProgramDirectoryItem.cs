namespace StartMenuCleaner
{
	public class ProgramDirectoryItem
	{
		public string Path { get; private set; }

		public CleanReason Reason { get; private set; }

		public ProgramDirectoryItem(string path, CleanReason reason)
		{
			this.Path = path;
			this.Reason = reason;
		}
	}
}