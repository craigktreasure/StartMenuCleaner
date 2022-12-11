namespace StartMenuCleaner.Tests.Cleaners.Directory;

using StartMenuCleaner.Cleaners.Directory;

public class FewAppsWithCruftDirectoryCleanerTests : DirectoryCleanerTestBase
{
    private readonly ITestOutputHelper output;

    public FewAppsWithCruftDirectoryCleanerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void CanClean_Help()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_OtherFile()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
            $@"{directoryPath}\MyApp Help.txt",
            $@"{directoryPath}\Foo.other",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanClean_SingleApp()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_Text()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp Help.txt",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_ThreeApps()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            $@"{directoryPath}\MyApp3.lnk;C:\Programs\MyApp\MyApp3.exe",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanClean_TwoApps()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_TwoAppsHelpAndText()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
            $@"{directoryPath}\MyApp Help.txt",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        bool result = cleaner.CanClean(directoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Clean_Help()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        string filePath = $@"{directoryPath}\MyApp Help.lnk";
        this.FileSystemComposer.Add(new[]
        {
            $@"{filePath};C:\Programs\MyApp\MyApp Help.chm",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        cleaner.Clean(directoryPath);

        // Assert
        Assert.False(this.FileSystem.FileExists(filePath));
    }

    [Fact]
    public void Clean_OtherFile()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
            $@"{directoryPath}\MyApp Help.txt",
            $@"{directoryPath}\Foo.other",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => cleaner.Clean(directoryPath));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\MyApp.lnk"));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\MyApp2.lnk"));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\MyApp Help.lnk"));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\MyApp Help.txt"));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\Foo.other"));
    }

    [Fact]
    public void Clean_SingleApp()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        string filePath = $@"{directoryPath}\MyApp.lnk";
        this.FileSystemComposer.Add(new[]
        {
            $@"{filePath};C:\Programs\MyApp\MyApp.exe",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        cleaner.Clean(directoryPath);

        // Assert
        Assert.False(this.FileSystem.FileExists(filePath));
    }

    [Fact]
    public void Clean_Text()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        string filePath = $@"{directoryPath}\MyApp Help.txt";
        this.FileSystemComposer.Add(new[]
        {
            filePath,
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        cleaner.Clean(directoryPath);

        // Assert
        Assert.False(this.FileSystem.FileExists(filePath));
    }

    [Fact]
    public void Clean_ThreeApps()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            $@"{directoryPath}\MyApp3.lnk;C:\Programs\MyApp\MyApp3.exe",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => cleaner.Clean(directoryPath));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\MyApp.lnk"));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\MyApp2.lnk"));
        Assert.True(this.FileSystem.FileExists($@"{directoryPath}\MyApp3.lnk"));
    }

    [Fact]
    public void Clean_TwoApps()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        cleaner.Clean(directoryPath);

        // Assert
        Assert.False(this.FileSystem.FileExists($@"{directoryPath}\MyApp.lnk"));
        Assert.False(this.FileSystem.FileExists($@"{directoryPath}\MyApp2.lnk"));
    }

    [Fact]
    public void Clean_TwoAppsHelpAndText()
    {
        // Arrange
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.FileSystemComposer.Add(new[]
        {
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
            $@"{directoryPath}\MyApp Help.txt",
        });
        IDirectoryCleaner cleaner = this.BuildCleaner();

        // Act
        cleaner.Clean(directoryPath);

        // Assert
        Assert.False(this.FileSystem.FileExists($@"{directoryPath}\MyApp.lnk"));
        Assert.False(this.FileSystem.FileExists($@"{directoryPath}\MyApp2.lnk"));
        Assert.False(this.FileSystem.FileExists($@"{directoryPath}\MyApp Help.lnk"));
        Assert.False(this.FileSystem.FileExists($@"{directoryPath}\MyApp Help.txt"));
    }

    private protected override IDirectoryCleaner BuildCleaner()
    {
        CleanerOptions cleanerOptions = new(new[] { @"C:\StartMenu" })
        {
            Simulate = false
        };
        FileClassifier fileClassifier = new(
            this.FileSystem,
            this.FileSystemComposer.ShortcutHandler);
        FewAppsWithCruftDirectoryCleaner cleaner = new(
            this.FileSystem,
            new FileSystemOperationHandler(
                this.output.BuildLoggerFor<FileSystemOperationHandler>(),
                this.FileSystem,
                cleanerOptions),
            fileClassifier);

        return cleaner;
    }
}
