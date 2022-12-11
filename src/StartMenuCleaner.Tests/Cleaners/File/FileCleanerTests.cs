namespace StartMenuCleaner.Tests.Cleaners.File;

using StartMenuCleaner.Cleaners.File;
using StartMenuCleaner.TestLibrary;

public class FileCleanerTests
{
    private readonly MockFileSystemComposer fileSystemComposer = new();

    [Fact]
    public void GetItemsToClean_Multiple()
    {
        // Arrange
        this.fileSystemComposer.Add($@"C:\StartMenu\Shortcut.lnk;C:\Programs\App.exe");
        FileCleaner fileCleaner = this.BuildFileCleaner();
        string[] directoryPaths = new[]
        {
            string.Empty,
            @"C:\StartMenu"
        };

        // Act
        IReadOnlyList<FileItemToClean> itemsToClean = fileCleaner.GetItemsToClean(directoryPaths);

        // Assert
        Assert.Single(itemsToClean);
    }

    [Fact]
    public void GetItemsToClean_Single()
    {
        // Arrange
        this.fileSystemComposer.Add($@"C:\StartMenu\Shortcut.lnk;C:\Programs\App.exe");
        FileCleaner fileCleaner = this.BuildFileCleaner();

        // Act
        IReadOnlyList<FileItemToClean> itemsToClean = fileCleaner.GetItemsToClean(@"C:\StartMenu");

        // Assert
        Assert.Single(itemsToClean);
    }

    [Fact]
    public void GetItemsToClean_Single_NotFound()
    {
        // Arrange
        FileCleaner fileCleaner = this.BuildFileCleaner();

        // Act and assert
        Assert.Throws<DirectoryNotFoundException>(() => fileCleaner.GetItemsToClean(string.Empty));
    }

    private FileCleaner BuildFileCleaner()
        => new(this.fileSystemComposer.FileSystem, new[] { new TestFileCleaner() });

    private sealed class TestFileCleaner : IFileCleaner
    {
        public FileCleanerType CleanerType => (FileCleanerType)(-1);

        public bool CanClean(string _) => true;

        public void Clean(string _) { }
    }
}
