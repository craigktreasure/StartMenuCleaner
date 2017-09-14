namespace StartMenuCleaner
{
	public class ProgramDirectoryItem
	{
		public string Path { get; }

		public CleanReason Reason { get; }

		public ProgramDirectoryItem(string path, CleanReason reason)
		{
			this.Path = path;
			this.Reason = reason;
		}
	}
}