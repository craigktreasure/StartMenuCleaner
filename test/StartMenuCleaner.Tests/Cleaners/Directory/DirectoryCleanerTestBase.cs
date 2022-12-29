namespace StartMenuCleaner.Tests.Cleaners.Directory;

using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.TestLibrary;
using System.IO.Abstractions.TestingHelpers;

public abstract class DirectoryCleanerTestBase
{
    protected MockFileSystem FileSystem => this.FileSystemComposer.FileSystem;

    protected MockFileSystemComposer FileSystemComposer { get; } = new MockFileSystemComposer();

    [Fact]
    public void CanClean_DirectoryNotFound()
    {
        // Arrange
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<DirectoryNotFoundException>(() => cleaner.CanClean(string.Empty));
    }

    [Fact]
    public void Clean_DirectoryNotFound()
    {
        // Arrange
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<DirectoryNotFoundException>(() => cleaner.Clean(string.Empty));
    }

    private protected abstract IDirectoryCleaner BuildCleaner();
}
