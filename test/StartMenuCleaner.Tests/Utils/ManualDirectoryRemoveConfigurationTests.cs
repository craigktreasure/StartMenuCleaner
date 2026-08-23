namespace StartMenuCleaner.Tests.Utils;

using StartMenuCleaner.Utils;

public class ManualDirectoryRemoveConfigurationTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange
        const string directoryName = "filename";
        IReadOnlySet<string> filesToPromote = new HashSet<string>();

        // Act and assert
        ManualDirectoryRemoveConfiguration config = new(directoryName, filesToPromote);
        Assert.Equal(directoryName, config.DirectoryName);

        Assert.Throws<ArgumentNullException>(nameof(directoryName), () => new ManualDirectoryRemoveConfiguration(null!, filesToPromote));
        Assert.Throws<ArgumentException>(nameof(directoryName), () => new ManualDirectoryRemoveConfiguration(string.Empty, filesToPromote));
        Assert.Throws<ArgumentException>(nameof(directoryName), () => new ManualDirectoryRemoveConfiguration(" ", filesToPromote));

        Assert.Throws<ArgumentNullException>(nameof(filesToPromote), () => new ManualDirectoryRemoveConfiguration(directoryName, null!));
    }
}
