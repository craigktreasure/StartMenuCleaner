namespace StartMenuCleaner.Tests;

using StartMenuCleaner.TestLibrary;
using StartMenuCleaner.Utils;

public class CleanerOptionsTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange
        IReadOnlyList<string> rootFoldersToClean = ["foo"];
        IReadOnlyList<string> foldersToIgnore = ["foo", .. Constants.DirectoriesToIgnore];

        // Act and assert
        CleanerOptions options = new(rootFoldersToClean);
        Assert.Same(rootFoldersToClean, options.RootFoldersToClean);
        Assert.Equal(Constants.DirectoriesToIgnore, options.FoldersToIgnore.Order());

        options = new(rootFoldersToClean, foldersToIgnore);
        Assert.Same(rootFoldersToClean, options.RootFoldersToClean);
        Assert.Equal(foldersToIgnore.Order(), options.FoldersToIgnore.Order());

        Assert.Throws<ArgumentNullException>(nameof(rootFoldersToClean), () => new CleanerOptions(null!, foldersToIgnore));
        Assert.Throws<ArgumentNullException>(nameof(foldersToIgnore), () => new CleanerOptions(rootFoldersToClean, null!));
    }

    [Fact]
    public void Load()
    {
        // Arrange
        bool simulate = true;
        IReadOnlyList<string> expectedRootFoldersToClean = StartMenuHelper.GetKnownStartMenuProgramsFolders();
        string[] expectedFoldersToIgnore = [.. Constants.DirectoriesToIgnore, "App", "App 2"];
        EmbeddedResourceHelper embeddedResourceHelper = new(typeof(AssemblyToken).Assembly);
        using TemporaryFile tempFile = embeddedResourceHelper.ToTemporaryFile(@"EmbeddedContent\options.netconfig");

        // Act
        CleanerOptions options = CleanerOptions.Load(simulate, tempFile.FilePath);

        // Assert
        Assert.Equal(simulate, options.Simulate);
        Assert.Equal(expectedRootFoldersToClean, options.RootFoldersToClean);
        Assert.Equal(expectedFoldersToIgnore, options.FoldersToIgnore);
    }
}
