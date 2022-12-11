namespace StartMenuCleaner.Tests;

using StartMenuCleaner.TestLibrary;
using Xunit;

public class CleanupRulesEngineTests
{
    private readonly CleanupRulesEngine cleanupEngine;

    private readonly MockFileSystemComposer fileSystemComposer = new();

    public CleanupRulesEngineTests()
    {
        FileClassifier classifier = new(this.fileSystemComposer.FileSystem, this.fileSystemComposer.ShortcutHandler);
        this.cleanupEngine = new CleanupRulesEngine(this.fileSystemComposer.FileSystem, classifier);
    }

    [Fact]
    public void TestFewAppsWithCruft()
    {
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.fileSystemComposer.Add(new[]
        {
                $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
                $@"{directoryPath}\MyApp Help.txt",
            });

        CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

        Assert.Equal(CleanReason.FewAppsWithCruft, actual);
    }

    [Fact]
    public void TestFewAppsWithCruftWithDirectory()
    {
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.fileSystemComposer.Add(new[]
        {
                $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
                $@"{directoryPath}\MyApp Help.txt",
                $@"{directoryPath}\Foo",
            });

        CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

        Assert.Equal(CleanReason.None, actual);
    }

    [Fact]
    public void TestFewAppsWithCruftWithOtherFile()
    {
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.fileSystemComposer.Add(new[]
        {
                $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                $@"{directoryPath}\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
                $@"{directoryPath}\MyApp Help.txt",
                $@"{directoryPath}\Foo.other",
            });

        CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

        Assert.Equal(CleanReason.None, actual);
    }

    [Fact]
    public void TestIgnoreFolderName()
    {
        const string directoryPath = @"C:\StartMenu\Maintenance";
        this.fileSystemComposer.Add(
            $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe"
        );

        CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

        Assert.Equal(CleanReason.None, actual);
    }

    [Fact]
    public void TestSingleAppWithExtraDirectory()
    {
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.fileSystemComposer.Add(new[]
        {
                $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                $@"{directoryPath}\Foo",
            });

        CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

        Assert.Equal(CleanReason.None, actual);
    }

    [Fact]
    public void TestThreeApps()
    {
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.fileSystemComposer.Add(new[]
        {
                $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                $@"{directoryPath}\MyApp3.lnk;C:\Programs\MyApp\MyApp3.exe",
            });

        CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

        Assert.Equal(CleanReason.None, actual);
    }

    [Fact]
    public void TestTwoApps()
    {
        const string directoryPath = @"C:\StartMenu\MyApp";
        this.fileSystemComposer.Add(new[]
        {
                $@"{directoryPath}\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                $@"{directoryPath}\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe"
            });

        CleanReason actual = this.cleanupEngine.TestForCleanReason(directoryPath);

        Assert.Equal(CleanReason.FewAppsWithCruft, actual);
    }
}
