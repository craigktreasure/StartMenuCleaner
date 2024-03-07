namespace StartMenuCleaner.Tests.Cleaners.Directory;

using StartMenuCleaner.Cleaners;
using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.TestLibrary;

public class ManualConfigurationDirectoryCleanerTests : DirectoryCleanerTestBase
{
    private readonly ITestOutputHelper output;

    public ManualConfigurationDirectoryCleanerTests(ITestOutputHelper output)
        : base()
    {
        this.output = output;
    }

    [Fact]
    public void CanClean_DirectoryConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\App\Shortcut.lnk";
        string shortcutDirectoryPath = Path.GetDirectoryName(shortcutFilePath)!;
        const string appFilePath = @"C:\Programs\App.exe";
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");
        ManualConfigurationDirectoryCleaner directoryCleaner = this.BuildCleaner(["App"]);

        // Act
        bool result = directoryCleaner.CanClean(shortcutDirectoryPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanClean_DirectoryNotConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\App\Shortcut.lnk";
        string shortcutDirectoryPath = Path.GetDirectoryName(shortcutFilePath)!;
        const string appFilePath = @"C:\Programs\App.exe";
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");
        IDirectoryCleaner directoryCleaner = this.BuildCleaner();

        // Act
        bool result = directoryCleaner.CanClean(shortcutDirectoryPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Clean_DirectoryConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\App\Shortcut.lnk";
        string shortcutDirectoryPath = Path.GetDirectoryName(shortcutFilePath)!;
        const string appFilePath = @"C:\Programs\App.exe";
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");
        ManualConfigurationDirectoryCleaner directoryCleaner = this.BuildCleaner(["App"]);

        // Act
        directoryCleaner.Clean(shortcutDirectoryPath);

        // Assert
        Assert.False(this.FileSystem.Directory.Exists(shortcutDirectoryPath));
    }

    [Fact]
    public void Clean_DirectoryConfigured_WithPromotions()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\App\Shortcut.lnk";
        string shortcutDirectoryPath = Path.GetDirectoryName(shortcutFilePath)!;
        const string appFilePath = @"C:\Programs\App.exe";
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");
        this.FileSystemComposer.Add(@"C:\StartMenu\App\File.txt");
        ManualConfigurationDirectoryCleaner directoryCleaner = this.BuildCleaner(new Dictionary<string, IEnumerable<string>?>
        {
            ["App"] = [Path.GetFileName(shortcutFilePath)]
        });

        // Act
        directoryCleaner.Clean(shortcutDirectoryPath);

        // Assert
        Assert.False(this.FileSystem.FileExists(shortcutFilePath));
        Assert.True(this.FileSystem.FileExists(@"C:\StartMenu\Shortcut.lnk"));
        Assert.False(this.FileSystem.FileExists(@"C:\StartMenu\File.txt"));
    }

    [Fact]
    public void Clean_DirectoryNotConfigured()
    {
        // Arrange
        const string shortcutFilePath = @"C:\StartMenu\App\Shortcut.lnk";
        string shortcutDirectoryPath = Path.GetDirectoryName(shortcutFilePath)!;
        const string appFilePath = @"C:\Programs\App.exe";
        this.FileSystemComposer.Add($"{shortcutFilePath};{appFilePath}");
        IDirectoryCleaner directoryCleaner = this.BuildCleaner();

        // Act and assert
        Assert.Throws<InvalidOperationException>(() => directoryCleaner.Clean(shortcutDirectoryPath));
    }

    [Fact]
    public void Constructor()
    {
        // Act
        IDirectoryCleaner directoryCleaner = this.BuildCleaner();

        // Assert
        Assert.Equal(CleanerType.ManualConfiguration, directoryCleaner.CleanerType);
    }

    private protected override IDirectoryCleaner BuildCleaner()
        => this.BuildCleaner((IDictionary<string, IEnumerable<string>?>?)null);

    private ManualConfigurationDirectoryCleaner BuildCleaner(IDictionary<string, IEnumerable<string>?>? directories)
    {
        CleanerOptions cleanerOptions = new([@"C:\StartMenu"])
        {
            Simulate = false
        };
        ManualConfigurationDirectoryCleaner cleaner = new(
            this.FileSystem,
            new FileSystemOperationHandler(
                this.output.BuildLoggerFor<FileSystemOperationHandler>(),
                this.FileSystem,
                cleanerOptions),
            new MockManualConfigurationLoader(directories: directories));

        return cleaner;
    }

    private ManualConfigurationDirectoryCleaner BuildCleaner(IEnumerable<string> directories)
        => this.BuildCleaner(directories.ToDictionary(d => d, _ => (IEnumerable<string>?)[], StringComparer.OrdinalIgnoreCase));
}
