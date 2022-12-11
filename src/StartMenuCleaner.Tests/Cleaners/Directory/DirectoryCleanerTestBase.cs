namespace StartMenuCleaner.Tests.Cleaners.Directory;

using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.TestLibrary;
using System.IO.Abstractions.TestingHelpers;

public abstract class DirectoryCleanerTestBase
{
    public static IEnumerable<object[]> DirectoriesToIgnoreTestData => Constants.DirectoriesToIgnore.Select(d => new object[] { d });

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

    [Theory]
    [MemberData(nameof(DirectoriesToIgnoreTestData))]
    public void CanClean_IgnoredDirectory(string directoryPath)
    {
        // Arrange
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(directoryPath);

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Clean_DirectoryNotFound()
    {
        // Arrange
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<DirectoryNotFoundException>(() => cleaner.Clean(string.Empty));
    }

    [Theory]
    [MemberData(nameof(DirectoriesToIgnoreTestData))]
    public void Clean_IgnoredDirectory(string directoryPath)
    {
        // Arrange
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(directoryPath);

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => cleaner.Clean(directoryPath));
        Assert.True(this.FileSystem.Directory.Exists(directoryPath));
    }

    private protected abstract IDirectoryCleaner BuildCleaner();
}
