namespace StartMenuCleaner.Tests.Utils;

using StartMenuCleaner.TestLibrary;
using StartMenuCleaner.Utils;

public class DotNetConfigManualConfigurationLoaderTests
{
    [Fact]
    public void LoadDirectoryConfigurations()
    {
        // Arrange
        EmbeddedResourceHelper embeddedResourceHelper = new(typeof(AssemblyToken).Assembly);
        using TemporaryFile tempFile = embeddedResourceHelper.ToTemporaryFile(@"EmbeddedContent\directories.netconfig");
        DotNetConfigManualConfigurationLoader loader = new(tempFile.FilePath);

        // Act
        IDictionary<string, ManualDirectoryRemoveConfiguration> configurations = loader.LoadDirectoryConfigurations();

        // Assert
        Assert.True(configurations.TryGetValue("Directory", out ManualDirectoryRemoveConfiguration? directoryConfig));
        Assert.NotNull(directoryConfig);
        Assert.Equal("Directory", directoryConfig.DirectoryName);
        Assert.Empty(directoryConfig.FilesToPromote);

        Assert.False(configurations.TryGetValue("Directory with remove disabled", out ManualDirectoryRemoveConfiguration? directoryWithRemoveDisabledConfig));
        Assert.Null(directoryWithRemoveDisabledConfig);

        Assert.False(configurations.TryGetValue("Directory with missing remove variable", out ManualDirectoryRemoveConfiguration? directoryWithMissingRemoveVarConfig));
        Assert.Null(directoryWithMissingRemoveVarConfig);

        Assert.False(configurations.TryGetValue("Directory with missing trailing slash", out ManualDirectoryRemoveConfiguration? directoryWithMissingTrailingSlashConfig));
        Assert.Null(directoryWithMissingTrailingSlashConfig);

        Assert.True(configurations.TryGetValue("Directory with promotions", out ManualDirectoryRemoveConfiguration? directoryWithPromotionsConfig));
        Assert.NotNull(directoryWithPromotionsConfig);
        Assert.Equal("Directory with promotions", directoryWithPromotionsConfig.DirectoryName);
        Assert.NotEmpty(directoryWithPromotionsConfig.FilesToPromote);
        Assert.Equal(2, directoryWithPromotionsConfig.FilesToPromote.Count);
        Assert.Contains("App1.lnk", directoryWithPromotionsConfig.FilesToPromote);
        Assert.Contains("App2.lnk", directoryWithPromotionsConfig.FilesToPromote);

        Assert.True(configurations.TryGetValue("Directory with duplicate promotions", out ManualDirectoryRemoveConfiguration? directoryWithDuplicatePromotionsConfig));
        Assert.NotNull(directoryWithDuplicatePromotionsConfig);
        Assert.Equal("Directory with duplicate promotions", directoryWithDuplicatePromotionsConfig.DirectoryName);
        Assert.Equal("App1.lnk", Assert.Single(directoryWithDuplicatePromotionsConfig.FilesToPromote));

        Assert.True(configurations.TryGetValue("Directory with promotion in subfolder", out ManualDirectoryRemoveConfiguration? directoryWithPromotionInSubfolderConfig));
        Assert.NotNull(directoryWithPromotionInSubfolderConfig);
        Assert.Equal("Directory with promotion in subfolder", directoryWithPromotionInSubfolderConfig.DirectoryName);
        Assert.Equal("Subfolder/App1.lnk", Assert.Single(directoryWithPromotionInSubfolderConfig.FilesToPromote));

        Assert.Equal(4, configurations.Count);
    }

    [Fact]
    public void LoadFileConfigurations()
    {
        // Arrange
        EmbeddedResourceHelper embeddedResourceHelper = new(typeof(AssemblyToken).Assembly);
        using TemporaryFile tempFile = embeddedResourceHelper.ToTemporaryFile(@"EmbeddedContent\files.netconfig");
        DotNetConfigManualConfigurationLoader loader = new(tempFile.FilePath);

        // Act
        IDictionary<string, ManualFileRemoveConfiguration> configurations = loader.LoadFileConfigurations();

        // Assert
        (string fileName, ManualFileRemoveConfiguration fileConfig) = Assert.Single(configurations);
        Assert.NotNull(fileConfig);
        Assert.Equal("File", fileConfig.FileName);
    }
}
