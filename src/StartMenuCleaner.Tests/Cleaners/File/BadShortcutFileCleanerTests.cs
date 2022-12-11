namespace StartMenuCleaner.Tests.Cleaners.File;

using StartMenuCleaner.Cleaners.File;
using StartMenuCleaner.TestLibrary;

public class BadShortcutFileCleanerTests
{
    private readonly MockFileSystemComposer fileSystemComposer = new();

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
        BadShortcutFileCleaner fileCleaner = this.BuildFileCleaner();
        this.fileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");

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
        BadShortcutFileCleaner fileCleaner = this.BuildFileCleaner();
        this.fileSystemComposer.Add($"{shortcutFilePath};");

        // Act
        bool result = fileCleaner.CanClean(shortcutFilePath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanClean_FileNotExist()
    {
        // Arrange
        BadShortcutFileCleaner fileCleaner = this.BuildFileCleaner();

        // Act and assert
        Assert.Throws<FileNotFoundException>(() => fileCleaner.CanClean(string.Empty));
    }

    [Fact]
    public void CanClean_GoodShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        BadShortcutFileCleaner fileCleaner = this.BuildFileCleaner();
        this.fileSystemComposer.Add($"{shortcutFilePath};{appFilePath}", appFilePath);

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
        BadShortcutFileCleaner fileCleaner = this.BuildFileCleaner();
        this.fileSystemComposer.Add($@"{shortcutFilePath};C:\Programs\App.exe");

        // Act
        fileCleaner.Clean(shortcutFilePath);

        // Assert
        Assert.False(this.fileSystemComposer.FileSystem.FileExists(shortcutFilePath));
    }

    [Fact]
    public void Clean_EmptyShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        BadShortcutFileCleaner fileCleaner = this.BuildFileCleaner();
        this.fileSystemComposer.Add($"{shortcutFilePath};");

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => fileCleaner.Clean(shortcutFilePath));
        Assert.True(this.fileSystemComposer.FileSystem.FileExists(shortcutFilePath));
    }

    [Fact]
    public void Clean_GoodShortcut()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        BadShortcutFileCleaner fileCleaner = this.BuildFileCleaner();
        this.fileSystemComposer.Add($"{shortcutFilePath};{appFilePath}", appFilePath);

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => fileCleaner.Clean(shortcutFilePath));
        Assert.True(this.fileSystemComposer.FileSystem.FileExists(shortcutFilePath));
    }

    private BadShortcutFileCleaner BuildFileCleaner()
    {
        CleanerOptions cleanerOptions = new(new[] { @"C:\StartMenu" })
        {
            Simulate = false
        };
        BadShortcutFileCleaner fileCleaner = new(
            this.fileSystemComposer.FileSystem,
            this.fileSystemComposer.ShortcutHandler,
            new FileSystemOperationHandler(
                this.output.BuildLoggerFor<FileSystemOperationHandler>(),
                this.fileSystemComposer.FileSystem,
                cleanerOptions));

        return fileCleaner;
    }
}
