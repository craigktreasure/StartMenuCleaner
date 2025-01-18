namespace StartMenuCleaner.Tests.Utils;

using StartMenuCleaner.Utils;

using Xunit;

public class StartMenuHelperTests
{
    [Fact]
    public void GetKnownStartMenuFolders()
    {
        // Act
        IReadOnlyList<string> result = StartMenuHelper.GetKnownStartMenuFolders();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetKnownStartMenuProgramsFolders()
    {
        // Act
        IReadOnlyList<string> result = StartMenuHelper.GetKnownStartMenuProgramsFolders();

        // Assert
        Assert.Equal(2, result.Count);
    }
}
