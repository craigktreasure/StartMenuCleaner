namespace StartMenuCleaner.Tests.Cleaners.File;

using StartMenuCleaner.Cleaners.File;
using StartMenuCleaner.TestLibrary;

public class BadShortcutFileCleanerTests : FileCleanerTestBase
{
    private readonly ITestOutputHelper output;

    public BadShortcutFileCleanerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void CanClean_BadShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");

        // Act
        bool result = fileCleaner.CanClean(shortcutFilePath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_EmptyShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($"{shortcutFilePath};");

        // Act
        bool result = fileCleaner.CanClean(shortcutFilePath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanClean_GoodShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}", appFilePath);

        // Act
        bool result = fileCleaner.CanClean(shortcutFilePath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Clean_BadShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($@"{shortcutFilePath};C:\Programs\App.exe");

        // Act
        fileCleaner.Clean(shortcutFilePath);

        // Assert
        Assert.False(this.FileSystem.FileExists(shortcutFilePath));
    }

    [Fact]
    public void Clean_EmptyShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($"{shortcutFilePath};");

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => fileCleaner.Clean(shortcutFilePath));
        Assert.True(this.FileSystem.FileExists(shortcutFilePath));
    }

    [Fact]
    public void Clean_GoodShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}", appFilePath);

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => fileCleaner.Clean(shortcutFilePath));
        Assert.True(this.FileSystem.FileExists(shortcutFilePath));
    }

    private protected override IFileCleaner BuildCleaner()
    {
        CleanerOptions cleanerOptions = new(new[] { @"C:\StartMenu" })
        {
            Simulate = false
        };
        BadShortcutFileCleaner cleaner = new(
            this.FileSystem,
            this.FileSystemComposer.ShortcutHandler,
            new FileSystemOperationHandler(
                this.output.BuildLoggerFor<FileSystemOperationHandler>(),
                this.FileSystem,
                cleanerOptions));

        return cleaner;
    }
}
