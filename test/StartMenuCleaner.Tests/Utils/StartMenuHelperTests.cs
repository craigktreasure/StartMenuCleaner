namespace StartMenuCleaner.Tests.Utils;

using FluentAssertions;
using StartMenuCleaner.Utils;
using Xunit;

public class StartMenuHelperTests
{
    [Fact]
    public void GetKnownStartMenuFolders()
        => StartMenuHelper.GetKnownStartMenuFolders().Should().HaveCount(2);

    [Fact]
    public void GetKnownStartMenuProgramsFolders()
        => StartMenuHelper.GetKnownStartMenuProgramsFolders().Should().HaveCount(2);
}
