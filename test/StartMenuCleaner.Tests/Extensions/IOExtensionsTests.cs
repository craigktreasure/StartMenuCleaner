namespace StartMenuCleaner.Tests.Extensions;

using StartMenuCleaner.Extensions;
using StartMenuCleaner.TestLibrary;
using System.IO.Abstractions.TestingHelpers;

public class IOExtensionsTests
{
    [Theory]
    [InlineData(new[] { "/Root" }, "/Root", true)]
    [InlineData(new[] { "/Root/First" }, "/Root", false)]
    [InlineData(new[] { "/Root/First.txt" }, "/Root", false)]
    [InlineData(new[] { "/Root/First/Second" }, "/Root", false)]
    [InlineData(new[] { "/Root/First/Second/Third.txt" }, "/Root", false)]
    public void IsEmpty(string[] contents, string directoryPath, bool expectedResult)
    {
        ArgumentNullException.ThrowIfNull(contents);

        // Arrange
        MockFileSystem fileSystem = new();
        foreach (string path in contents)
        {
            if (fileSystem.Path.HasExtension(path))
            {
                fileSystem.AddEmptyFile(path);
            }
            else
            {
                fileSystem.AddDirectory(path);
            }
        }

        // Act
        bool result = fileSystem.Directory.IsEmpty(directoryPath);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
