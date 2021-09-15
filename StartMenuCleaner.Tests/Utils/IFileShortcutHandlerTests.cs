namespace StartMenuCleaner.Tests.Utils;

using FluentAssertions;
using StartMenuCleaner.TestLibrary;
using StartMenuCleaner.Utils;
using System;
using Xunit;

public class IFileShortcutHandlerTests
{
    private IFileShortcutHandler ShortcutHandler => this.testShortcutHandler;

    private readonly TestFileShortcutHandler testShortcutHandler = new();

    [Fact]
    public void IsShortcut()
    {
        this.ShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.lnk").Should().Be(true);
        this.ShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.txt").Should().Be(false);
    }

    [Fact]
    public void CreateShortcut()
    {
        FileShortcut expectedFileShortcut = new(@"C:\StartMenu\MyApp\MyApp.lnk", @"C:\Programs\MyApp\MyApp.exe");
        this.testShortcutHandler.AddShortcutMapping(expectedFileShortcut);

        FileShortcut shortcut = this.ShortcutHandler.CreateShortcut(expectedFileShortcut.FilePath);
        shortcut.Should().Be(expectedFileShortcut);
    }

    [Fact]
    public void CreateShortcutFromInvalidPath()
    {
        Assert.Throws<ArgumentException>("filePath", () =>
            this.ShortcutHandler.CreateShortcut(@"C:\StartMenu\MyApp\MyApp.txt"));
    }

    [Fact]
    public void TryCreateShortcut()
    {
        FileShortcut expectedFileShortcut = new(@"C:\StartMenu\MyApp\MyApp.lnk", @"C:\Programs\MyApp\MyApp.exe");
        this.testShortcutHandler.AddShortcutMapping(expectedFileShortcut);

        this.ShortcutHandler.TryCreateShortcut(expectedFileShortcut.FilePath, out FileShortcut? shortcut)
            .Should().Be(true);

        shortcut.Should().Be(expectedFileShortcut);
    }

    [Fact]
    public void TryCreateShortcutFromInvalidPath()
    {
        this.ShortcutHandler.TryCreateShortcut(@"C:\StartMenu\MyApp\MyApp.txt", out FileShortcut? shortcut)
            .Should().Be(false);

        shortcut.Should().BeNull();
    }

    [Fact]
    public void TryCreateShortcutWithResolutionFailure()
    {
        // Don't register the link causing the TestFileShortcutHandler to
        // throw in ResolveTarget.
        this.ShortcutHandler.TryCreateShortcut(@"C:\StartMenu\MyApp\MyApp.lnk", out FileShortcut? shortcut)
            .Should().Be(false);

        shortcut.Should().BeNull();
    }
}
