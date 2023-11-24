namespace StartMenuCleaner.Tests.Utils;

using System;

using FluentAssertions;

using StartMenuCleaner.TestLibrary;
using StartMenuCleaner.Utils;

using Xunit;

public class FileShortcutHandlerTests
{
    private readonly TestFileShortcutHandler shortcutHandler = new();

    [Fact]
    public void GetShortcut()
    {
        FileShortcut expectedFileShortcut = new(@"C:\StartMenu\MyApp\MyApp.lnk", @"C:\Programs\MyApp\MyApp.exe");
        this.shortcutHandler.AddShortcutMapping(expectedFileShortcut);

        FileShortcut shortcut = this.shortcutHandler.GetShortcut(expectedFileShortcut.FilePath);
        shortcut.Should().Be(expectedFileShortcut);
    }

    [Fact]
    public void GetShortcutFromInvalidPath()
    {
        Assert.Throws<ArgumentException>("filePath", () =>
            this.shortcutHandler.GetShortcut(@"C:\StartMenu\MyApp\MyApp.txt"));
    }

    [Fact]
    public void IsShortcut_ContainsNonShortcutExtension_ReturnsFalse()
    {
        FileShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.txt").Should().Be(false);
    }

    [Fact]
    public void IsShortcut_ContainsShortcutExtension_ReturnsTrue()
    {
        FileShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.lnk").Should().Be(true);
    }

    [Fact]
    public void TryGetShortcut()
    {
        FileShortcut expectedFileShortcut = new(@"C:\StartMenu\MyApp\MyApp.lnk", @"C:\Programs\MyApp\MyApp.exe");
        this.shortcutHandler.AddShortcutMapping(expectedFileShortcut);

        this.shortcutHandler.TryGetShortcut(expectedFileShortcut.FilePath, out FileShortcut? shortcut)
            .Should().Be(true);

        shortcut.Should().Be(expectedFileShortcut);
    }

    [Fact]
    public void TryGetShortcutFromInvalidPath()
    {
        this.shortcutHandler.TryGetShortcut(@"C:\StartMenu\MyApp\MyApp.txt", out FileShortcut? shortcut)
            .Should().Be(false);

        shortcut.Should().BeNull();
    }

    [Fact]
    public void TryGetShortcutWithResolutionFailure()
    {
        // Don't register the link causing the TestFileShortcutHandler to
        // throw in ResolveTarget.
        this.shortcutHandler.TryGetShortcut(@"C:\StartMenu\MyApp\MyApp.lnk", out FileShortcut? shortcut)
            .Should().Be(false);

        shortcut.Should().BeNull();
    }
}
