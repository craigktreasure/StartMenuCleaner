namespace StartMenuCleaner.Tests.Utils;

using FluentAssertions;
using StartMenuCleaner.TestLibrary;
using StartMenuCleaner.Utils;
using System;
using Xunit;

public class IFileShortcutHandlerTests
{
    private readonly TestFileShortcutHandler testShortcutHandler = new();

    private IFileShortcutHandler ShortcutHandler => this.testShortcutHandler;

    [Fact]
    public void GetShortcut()
    {
        FileShortcut expectedFileShortcut = new(@"C:\StartMenu\MyApp\MyApp.lnk", @"C:\Programs\MyApp\MyApp.exe");
        this.testShortcutHandler.AddShortcutMapping(expectedFileShortcut);

        FileShortcut shortcut = this.ShortcutHandler.GetShortcut(expectedFileShortcut.FilePath);
        shortcut.Should().Be(expectedFileShortcut);
    }

    [Fact]
    public void GetShortcutFromInvalidPath()
    {
        Assert.Throws<ArgumentException>("filePath", () =>
            this.ShortcutHandler.GetShortcut(@"C:\StartMenu\MyApp\MyApp.txt"));
    }

    [Fact]
    public void IsShortcut()
    {
        this.ShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.lnk").Should().Be(true);
        this.ShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.txt").Should().Be(false);
    }

    [Fact]
    public void TryGetShortcut()
    {
        FileShortcut expectedFileShortcut = new(@"C:\StartMenu\MyApp\MyApp.lnk", @"C:\Programs\MyApp\MyApp.exe");
        this.testShortcutHandler.AddShortcutMapping(expectedFileShortcut);

        this.ShortcutHandler.TryGetShortcut(expectedFileShortcut.FilePath, out FileShortcut? shortcut)
            .Should().Be(true);

        shortcut.Should().Be(expectedFileShortcut);
    }

    [Fact]
    public void TryGetShortcutFromInvalidPath()
    {
        this.ShortcutHandler.TryGetShortcut(@"C:\StartMenu\MyApp\MyApp.txt", out FileShortcut? shortcut)
            .Should().Be(false);

        shortcut.Should().BeNull();
    }

    [Fact]
    public void TryGetShortcutWithResolutionFailure()
    {
        // Don't register the link causing the TestFileShortcutHandler to
        // throw in ResolveTarget.
        this.ShortcutHandler.TryGetShortcut(@"C:\StartMenu\MyApp\MyApp.lnk", out FileShortcut? shortcut)
            .Should().Be(false);

        shortcut.Should().BeNull();
    }
}
