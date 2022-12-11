namespace StartMenuCleaner.Tests.Cleaners.Directory;

using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.TestLibrary;

public class DirectoryCleanerTests
{
    private readonly MockFileSystemComposer fileSystemComposer = new();

    [Fact]
    public void GetItemsToClean_Multiple()
    {
        // Arrange
        this.fileSystemComposer.Add($@"C:\StartMenu\Shortcut\Shortcut.lnk;C:\Programs\App.exe");
        DirectoryCleaner cleaner = this.BuildDirectoryCleaner();
        string[] directoryPaths = new[]
        {
            string.Empty,
            @"C:\StartMenu"
        };

        // Act
        IReadOnlyList<DirectoryItemToClean> itemsToClean = cleaner.GetItemsToClean(directoryPaths);

        // Assert
        Assert.Single(itemsToClean);
    }

    [Fact]
    public void GetItemsToClean_Single()
    {
        // Arrange
        this.fileSystemComposer.Add($@"C:\StartMenu\Shortcut\Shortcut.lnk;C:\Programs\App.exe");
        DirectoryCleaner cleaner = this.BuildDirectoryCleaner();

        // Act
        IReadOnlyList<DirectoryItemToClean> itemsToClean = cleaner.GetItemsToClean(@"C:\StartMenu");

        // Assert
        Assert.Single(itemsToClean);
    }

    [Fact]
    public void GetItemsToClean_Single_NotFound()
    {
        // Arrange
        DirectoryCleaner cleaner = this.BuildDirectoryCleaner();

        // Act and assert
        Assert.Throws<DirectoryNotFoundException>(() => cleaner.GetItemsToClean(string.Empty));
    }

    private DirectoryCleaner BuildDirectoryCleaner()
        => new(this.fileSystemComposer.FileSystem, new[] { new TestDirectoryCleaner() });

    private sealed class TestDirectoryCleaner : IDirectoryCleaner
    {
        public DirectoryCleanerType CleanerType => (DirectoryCleanerType)(-1);

        public bool CanClean(string _) => true;

        public void Clean(string _)
        { }
    }
}
