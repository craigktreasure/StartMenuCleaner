namespace StartMenuCleaner.Tests.Cleaners.Directory;

using StartMenuCleaner.Cleaners.Directory;

public class SingleAppDirectoryCleanerTests : DirectoryCleanerTestBase
{
    private readonly ITestOutputHelper output;

    public SingleAppDirectoryCleanerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void CanClean_ContainsDirectories()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(directoryPath, $@"{directoryPath}\Subfolder");

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.False(result);
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
        Assert.False(result);
    }

    [Fact]
    public void CanClean_MultipleApps()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(
            directoryPath,
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp2.exe");

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanClean_SingleApp()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(directoryPath, $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp.exe");

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_SingleNonApp()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\Folder";
        IDirectoryCleaner cleaner = this.BuildCleaner();
        this.FileSystemComposer.Add(
            directoryPath,
            $@"{directoryPath}\MyApp.lnk;C:\Programs\Foo.txt");

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.False(result);
    }

    private protected override IDirectoryCleaner BuildCleaner()
    {
        CleanerOptions cleanerOptions = new([@"C:\StartMenu"])
        {
            Simulate = false
        };
        FileClassifier fileClassifier = new(
            this.FileSystem,
            this.FileSystemComposer.ShortcutHandler);
        SingleAppDirectoryCleaner cleaner = new(
            this.FileSystem,
            new FileSystemOperationHandler(
                this.output.BuildLoggerFor<FileSystemOperationHandler>(),
                this.FileSystem,
                cleanerOptions),
            fileClassifier);

        return cleaner;
    }
}
