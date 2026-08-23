namespace StartMenuCleaner.Tests.Utils;

using StartMenuCleaner.Utils;

public class ManualFileRemoveConfigurationTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange
        const string fileName = "filename";

        // Act and assert
        ManualFileRemoveConfiguration config = new(fileName);
        Assert.Equal(fileName, config.FileName);

        Assert.Throws<ArgumentNullException>(nameof(fileName), () => new ManualFileRemoveConfiguration(null!));
        Assert.Throws<ArgumentException>(nameof(fileName), () => new ManualFileRemoveConfiguration(string.Empty));
        Assert.Throws<ArgumentException>(nameof(fileName), () => new ManualFileRemoveConfiguration(" "));
    }
}
