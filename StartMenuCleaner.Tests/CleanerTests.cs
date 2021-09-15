namespace StartMenuCleaner.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using StartMenuCleaner.TestLibrary;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Xunit.Abstractions;

public class CleanerTests
{
    private readonly MockFileSystemComposer fileSystemComposer = new();

    private readonly ITestOutputHelper output;

    public CleanerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void CleanEmptyFolder()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(@"C:\StartMenu\MyApp");

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu"
            });
    }

    [Fact]
    public void CleanFewAppsWithCruft()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
                @"C:\StartMenu\MyApp\MyApp Help.txt",
            });

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp.lnk",
                @"C:\StartMenu\MyApp2.lnk",
            });
    }

    [Fact]
    public void CleanFewAppsWithCruftWithDirectory()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
                @"C:\StartMenu\MyApp\MyApp Help.txt",
                @"C:\StartMenu\MyApp\Foo",
            });

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk",
                @"C:\StartMenu\MyApp\MyApp2.lnk",
                @"C:\StartMenu\MyApp\MyApp Help.lnk",
                @"C:\StartMenu\MyApp\MyApp Help.txt",
                @"C:\StartMenu\MyApp\Foo",
            });
    }

    [Fact]
    public void CleanFewAppsWithCruftWithOtherFile()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                @"C:\StartMenu\MyApp\MyApp Help.lnk;C:\Programs\MyApp\MyApp Help.chm",
                @"C:\StartMenu\MyApp\MyApp Help.txt",
                @"C:\StartMenu\MyApp\Foo.other",
            });

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk",
                @"C:\StartMenu\MyApp\MyApp2.lnk",
                @"C:\StartMenu\MyApp\MyApp Help.lnk",
                @"C:\StartMenu\MyApp\MyApp Help.txt",
                @"C:\StartMenu\MyApp\Foo.other",
            });
    }

    [Fact]
    public void CleanIgnoredFolderName()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
            @"C:\StartMenu\Maintenance\MyApp.lnk;C:\Programs\MyApp\MyApp.exe"
        );

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\Maintenance\MyApp.lnk"
            });
    }

    [Fact]
    public void CleanSingleApp()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(
            @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe"
        );

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp.lnk"
            });
    }

    [Fact]
    public void CleanSingleAppWithExtraDirectory()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                @"C:\StartMenu\MyApp\Foo",
            });

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk",
                @"C:\StartMenu\MyApp\Foo"
            });
    }

    [Fact]
    public void CleanThreeApps()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
                @"C:\StartMenu\MyApp\MyApp3.lnk;C:\Programs\MyApp\MyApp3.exe",
            });

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk",
                @"C:\StartMenu\MyApp\MyApp2.lnk",
                @"C:\StartMenu\MyApp\MyApp3.lnk",
            });
    }

    [Fact]
    public void CleanTwoApps()
    {
        Cleaner cleaner = this.GetCleaner();

        this.fileSystemComposer.Add(new[]
        {
                @"C:\StartMenu\MyApp\MyApp.lnk;C:\Programs\MyApp\MyApp.exe",
                @"C:\StartMenu\MyApp\MyApp2.lnk;C:\Programs\MyApp\MyApp2.exe",
            });

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp.lnk",
                @"C:\StartMenu\MyApp2.lnk",
            });
    }

    [Fact]
    public void CleanWithSimulate()
    {
        Cleaner cleaner = this.GetCleaner(simulate: true);

        this.fileSystemComposer.Add(@"C:\StartMenu\MyApp");

        cleaner.Start();

        this.AssertFileSystemContains(new[]
        {
                @"C:\StartMenu\MyApp"
            });
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
        CleanupRulesEngine cleanupEngine = new(mockFileSystem, classifier);
        CleanerOptions cleanerOptions = new(new[] { @"C:\StartMenu" })
        {
            Simulate = simulate
        };

        return new Cleaner(
            cleanerOptions,
            mockFileSystem,
            classifier,
            cleanupEngine,
            logger);
    }
}
