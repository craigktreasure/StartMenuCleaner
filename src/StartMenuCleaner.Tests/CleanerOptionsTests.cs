namespace StartMenuCleaner.Tests;

using StartMenuCleaner.TestLibrary;
using StartMenuCleaner.Utils;

public class CleanerOptionsTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange
        IReadOnlyList<string> rootFoldersToClean = new[] { "foo" };
        IReadOnlyList<string> foldersToIgnore = new[] { "foo" }.Concat(Constants.DirectoriesToIgnore).ToArray();

        // Act and assert
        CleanerOptions options = new(rootFoldersToClean);
        Assert.Same(rootFoldersToClean, options.RootFoldersToClean);
        Assert.Equal(Constants.DirectoriesToIgnore, options.FoldersToIgnore);

        options = new(rootFoldersToClean, foldersToIgnore);
        Assert.Same(rootFoldersToClean, options.RootFoldersToClean);
        Assert.Equal(foldersToIgnore, options.FoldersToIgnore);

        Assert.Throws<ArgumentNullException>(nameof(rootFoldersToClean), () => new CleanerOptions(null!, foldersToIgnore));
        Assert.Throws<ArgumentNullException>(nameof(foldersToIgnore), () => new CleanerOptions(rootFoldersToClean, null!));
    }

    [Fact]
    public void Load()
    {
        // Arrange
        bool simulate = true;
        IReadOnlyList<string> expectedRootFoldersToClean = StartMenuHelper.GetKnownStartMenuProgramsFolders();
        string[] expectedFoldersToIgnore = Constants.DirectoriesToIgnore.Concat(new[] { "App", "App 2" }).ToArray();
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
