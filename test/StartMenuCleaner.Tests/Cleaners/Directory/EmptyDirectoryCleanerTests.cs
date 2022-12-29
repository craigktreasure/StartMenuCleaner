namespace StartMenuCleaner.Tests.Cleaners.Directory;

using StartMenuCleaner.Cleaners.Directory;

public class EmptyDirectoryCleanerTests : DirectoryCleanerTestBase
{
    private readonly ITestOutputHelper output;

    public EmptyDirectoryCleanerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void CanClean_EmptyDirectory()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(directoryPath);

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_NonEmptyDirectory()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($@"{directoryPath}\something.txt");

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Clean_EmptyDirectory()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(directoryPath);

        // Act
        cleaner.Clean(directoryPath);

        // Assert
        Assert.False(this.FileSystemComposer.FileSystem.Directory.Exists(directoryPath));
    }

    [Fact]
    public void Clean_NonEmptyDirectory()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($@"{directoryPath}\something.txt");

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => cleaner.Clean(directoryPath));
        Assert.True(this.FileSystemComposer.FileSystem.Directory.Exists(directoryPath));
    }

    private protected override IDirectoryCleaner BuildCleaner()
    {
        CleanerOptions cleanerOptions = new(new[] { @"C:\StartMenu" })
        {
            Simulate = false
        };
        EmptyDirectoryCleaner cleaner = new(
            this.FileSystem,
            new FileSystemOperationHandler(
                this.output.BuildLoggerFor<FileSystemOperationHandler>(),
                this.FileSystem,
                cleanerOptions));

        return cleaner;
    }
}
