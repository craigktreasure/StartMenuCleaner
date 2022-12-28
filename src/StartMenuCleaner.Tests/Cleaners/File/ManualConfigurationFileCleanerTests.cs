namespace StartMenuCleaner.Tests.Cleaners.File;

using StartMenuCleaner.Cleaners;
using StartMenuCleaner.Cleaners.File;
using StartMenuCleaner.TestLibrary;
using System.Collections.Generic;

public class ManualConfigurationFileCleanerTests : FileCleanerTestBase
{
    private readonly ITestOutputHelper output;

    public ManualConfigurationFileCleanerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void CanClean_FileConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        IFileCleaner fileCleaner = this.BuildCleaner(new[] { "Shortcut.lnk" });
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");

        // Act
        bool result = fileCleaner.CanClean(shortcutFilePath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_FileNotConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");

        // Act
        bool result = fileCleaner.CanClean(shortcutFilePath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Clean_FileConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        IFileCleaner fileCleaner = this.BuildCleaner(new[] { "Shortcut.lnk" });
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");

        // Act
        fileCleaner.Clean(shortcutFilePath);

        // Assert
        Assert.False(this.FileSystemComposer.FileSystem.FileExists(shortcutFilePath));
    }

    [Fact]
    public void Clean_FileNotConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\Shortcut.lnk";
        const string appFilePath = @"C:\Programs\App.exe";
        IFileCleaner fileCleaner = this.BuildCleaner();
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => fileCleaner.Clean(shortcutFilePath));
    }

    [Fact]
    public void Constructor()
    {
        // Act
        IFileCleaner fileCleaner = this.BuildCleaner();

        // Assert
        Assert.Equal(CleanerType.ManualConfiguration, fileCleaner.CleanerType);
    }

    private protected override IFileCleaner BuildCleaner() => this.BuildCleaner(null);

    private IFileCleaner BuildCleaner(IEnumerable<string>? files)
    {
        CleanerOptions cleanerOptions = new(new[] { @"C:\StartMenu" })
        {
            Simulate = false
        };
        ManualConfigurationFileCleaner cleaner = new(
            this.FileSystem,
            new FileSystemOperationHandler(
                this.output.BuildLoggerFor<FileSystemOperationHandler>(),
                this.FileSystem,
                cleanerOptions),
            new MockManualConfigurationLoader(files));

        return cleaner;
    }
}
