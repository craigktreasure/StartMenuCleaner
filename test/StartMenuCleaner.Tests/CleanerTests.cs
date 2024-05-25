namespace StartMenuCleaner.Tests;

using System.IO.Abstractions.TestingHelpers;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using StartMenuCleaner.Cleaners.Directory;
using StartMenuCleaner.Cleaners.File;
using StartMenuCleaner.TestLibrary;

public class CleanerTests
{
    private readonly MockFileSystemComposer fileSystemComposer = new();

    private readonly ITestOutputHelper output;

    public CleanerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void BadShortcut()
    {
        // Arrange
        Cleaner cleaner = this.GetCleaner();
        this.fileSystemComposer.Add(@"C:\StartMenu\MyApp.lnk;C:\Programs\MyApp.exe");

        // Act
        cleaner.Start();

        // Assert
        this.AssertFileSystemContains([@"C:\StartMenu"]);
    }

    [Fact]
    public void CleanEmptyFolder()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(@"C:\StartMenu\MyApp");

        cleaner.Start();

        this.AssertFileSystemContains([@"C:\StartMenu"]);
    }

    [Fact]
    public void CleanFewAppsWithCruft()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
            @"C:\StartMenu\MyApp\MyApp Help.txt",
        ]);

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\MyApp.lnk",
            @"C:\StartMenu\MyApp2.lnk",
        ]);
    }

    [Fact]
    public void CleanFewAppsWithCruftWithDirectory()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
            @"C:\StartMenu\MyApp\MyApp Help.txt",
            @"C:\StartMenu\MyApp\Foo",
        ]);

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk",
            @"C:\StartMenu\MyApp\MyApp2.lnk",
            @"C:\StartMenu\MyApp\MyApp Help.lnk",
            @"C:\StartMenu\MyApp\MyApp Help.txt",
            @"C:\StartMenu\MyApp\Foo",
        ]);
    }

    [Fact]
    public void CleanFewAppsWithCruftWithOtherFile()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
            @"C:\StartMenu\MyApp\MyApp Help.txt",
            @"C:\StartMenu\MyApp\Foo.other",
        ]);

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk",
            @"C:\StartMenu\MyApp\MyApp2.lnk",
            @"C:\StartMenu\MyApp\MyApp Help.lnk",
            @"C:\StartMenu\MyApp\MyApp Help.txt",
            @"C:\StartMenu\MyApp\Foo.other",
        ]);
    }

    [Fact]
    public void CleanIgnored()
    {
        Cleaner cleaner = this.GetCleaner();

        string[] ignoredDirectoryPaths = Constants.DirectoriesToIgnore.Select(d => $@"C:\StartMenu\{d}").ToArray();
        foreach (string ignoredDirectoryPath in ignoredDirectoryPaths)
        {
            this.fileSystemComposer.AddDirectory(ignoredDirectoryPath);
        }

        cleaner.Start();

        this.AssertFileSystemContains(ignoredDirectoryPaths);
    }

    [Fact]
    public void CleanIgnoredFolderName()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
            @"C:\StartMenu\Maintenance\MyApp.lnk;C:\Programs\MyApp\MyApp.exe"
        );

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\Maintenance\MyApp.lnk"
        ]);
    }

    [Fact]
    public void CleanSingleApp()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe"
        );

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\MyApp.lnk"
        ]);
    }

    [Fact]
    public void CleanSingleAppWithExtraDirectory()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            @"C:\StartMenu\MyApp\Foo",
        ]);

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk",
            @"C:\StartMenu\MyApp\Foo"
        ]);
    }

    [Fact]
    public void CleanThreeApps()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            @"C:\StartMenu\MyApp\MyApp3.lnk;C:\Programs\MyApp\MyApp3.exe",
        ]);

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk",
            @"C:\StartMenu\MyApp\MyApp2.lnk",
            @"C:\StartMenu\MyApp\MyApp3.lnk",
        ]);
    }

    [Fact]
    public void CleanTwoApps()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
        [
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
            @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
        ]);

        cleaner.Start();

        this.AssertFileSystemContains(
        [
            @"C:\StartMenu\MyApp.lnk",
            @"C:\StartMenu\MyApp2.lnk",
        ]);
    }

    [Fact]
    public void CleanWithSimulate()
    {
        Cleaner cleaner = this.GetCleaner(simulate: true);

        this.fileSystemComposer.Add(@"C:\StartMenu\MyApp");

        cleaner.Start();

        this.AssertFileSystemContains([@"C:\StartMenu\MyApp"]);
    }

    private void AssertFileSystemContains(IEnumerable<string> expectedNodes)
    {
        IEnumerable<string> fileSystemNodes = this.fileSystemComposer.GetAllNodes();
        expectedNodes.Should().BeEquivalentTo(fileSystemNodes);
    }

    private Cleaner GetCleaner(bool simulate = false)
    {
        ILogger<Cleaner> logger = this.output.BuildLoggerFor<Cleaner>(LogLevel.Trace);
        MockFileSystem mockFileSystem = this.fileSystemComposer.FileSystem;
        TestFileShortcutHandler shortcutHandler = this.fileSystemComposer.ShortcutHandler;
        FileClassifier classifier = new(mockFileSystem, shortcutHandler);
        CleanerOptions cleanerOptions = new([@"C:\StartMenu"])
        {
            Simulate = simulate
        };
        FileSystemOperationHandler fileSystemOperationHandler = new(
            this.output.BuildLoggerFor<FileSystemOperationHandler>(),
            mockFileSystem,
            cleanerOptions);
        IFileCleaner[] fileCleaners =
        [
            new BadShortcutFileCleaner(mockFileSystem, shortcutHandler, fileSystemOperationHandler),
        ];
        IDirectoryCleaner[] directoryCleaners =
        [
            new EmptyDirectoryCleaner(mockFileSystem, fileSystemOperationHandler),
            new SingleAppDirectoryCleaner(mockFileSystem, fileSystemOperationHandler, classifier),
            new FewAppsWithCruftDirectoryCleaner(mockFileSystem, fileSystemOperationHandler, classifier),
        ];

        return new Cleaner(
            cleanerOptions,
            logger,
            mockFileSystem,
            fileCleaners,
            directoryCleaners);
    }
}
