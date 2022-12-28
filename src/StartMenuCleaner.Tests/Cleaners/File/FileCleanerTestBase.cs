namespace StartMenuCleaner.Tests.Cleaners.File;

using StartMenuCleaner.Cleaners.File;
using StartMenuCleaner.TestLibrary;
using System.IO.Abstractions.TestingHelpers;

public abstract class FileCleanerTestBase
{
    protected MockFileSystem FileSystem => this.FileSystemComposer.FileSystem;

    protected MockFileSystemComposer FileSystemComposer { get; } = new MockFileSystemComposer();

    [Fact]
    public void CanClean_FileNotExist()
    {
        // Arrange
        IFileCleaner fileCleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<FileNotFoundException>(() => fileCleaner.CanClean(string.Empty));
    }

    [Fact]
    public void Clean_FileNotExist()
    {
        // Arrange
        IFileCleaner fileCleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<FileNotFoundException>(() => fileCleaner.Clean(string.Empty));
    }

    private protected abstract IFileCleaner BuildCleaner();
}
