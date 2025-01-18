namespace StartMenuCleaner.Tests.Utils;

using System;

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
        Assert.Equal(expectedFileShortcut, shortcut);
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
        // Act
        bool result = FileShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.txt");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsShortcut_ContainsShortcutExtension_ReturnsTrue()
    {
        // Act
        bool result = FileShortcutHandler.IsShortcut(@"C:\StartMenu\MyApp\MyApp.lnk");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TryGetShortcut()
    {
        // Arrange
        FileShortcut expectedFileShortcut = new(@"C:\StartMenu\MyApp\MyApp.lnk", @"C:\Programs\MyApp\MyApp.exe");
        this.shortcutHandler.AddShortcutMapping(expectedFileShortcut);

        // Act
        bool result = this.shortcutHandler.TryGetShortcut(expectedFileShortcut.FilePath, out FileShortcut? shortcut);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedFileShortcut, shortcut);
    }

    [Fact]
    public void TryGetShortcutFromInvalidPath()
    {
        // Act
        bool result = this.shortcutHandler.TryGetShortcut(@"C:\StartMenu\MyApp\MyApp.txt", out FileShortcut? shortcut);

        // Assert
        Assert.False(result);
        Assert.Null(shortcut);
    }

    [Fact]
    public void TryGetShortcutWithResolutionFailure()
    {
        // Act
        // Don't register the link causing the TestFileShortcutHandler to
        // throw in ResolveTarget.
        bool result = this.shortcutHandler.TryGetShortcut(@"C:\StartMenu\MyApp\MyApp.lnk", out FileShortcut? shortcut);

        // Assert
        Assert.False(result);
        Assert.Null(shortcut);
    }
}
