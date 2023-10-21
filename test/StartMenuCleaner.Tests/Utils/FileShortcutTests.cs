namespace StartMenuCleaner.Tests.Utils;

using System.Diagnostics.CodeAnalysis;

using StartMenuCleaner.Utils;

public sealed class FileShortcutTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange
        string filePath = @"C:\StartMenu\MyApp\MyApp.lnk";
        string targetPath = @"C:\Programs\MyApp\MyApp.exe";

        // Act
        FileShortcut fileShortcut = new(filePath, targetPath);

        // Assert
        Assert.Equal(filePath, fileShortcut.FilePath);
        Assert.Equal(targetPath, fileShortcut.TargetPath);
    }

    [Fact]
    [SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "<Pending>")]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        string filePath = @"C:\StartMenu\MyApp\MyApp.lnk";
        string targetPath = @"C:\Programs\MyApp\MyApp.exe";
        FileShortcut fileShortcut = new(filePath, targetPath);

        // Act
        bool result = fileShortcut.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ExplicitConversion_FromString()
    {
        // Arrange
        string filePath = @"C:\StartMenu\MyApp\MyApp.lnk";
        string targetPath = @"C:\Programs\MyApp\MyApp.exe";

        // Act
        FileShortcut fileShortcut = (FileShortcut)$"{filePath};{targetPath}";

        // Assert
        Assert.Equal(filePath, fileShortcut.FilePath);
        Assert.Equal(targetPath, fileShortcut.TargetPath);
    }

    [Fact]
    public void ExplicitConversion_ToString()
    {
        // Arrange
        string filePath = @"C:\StartMenu\MyApp\MyApp.lnk";
        string targetPath = @"C:\Programs\MyApp\MyApp.exe";
        string shortcutPathSyntax = $"{filePath};{targetPath}";
        FileShortcut fileShortcut = FileShortcut.FromString(shortcutPathSyntax);

        // Act
        string result = (string)fileShortcut;

        // Assert
        Assert.Equal(shortcutPathSyntax, result);
    }

    [Fact]
    public void FromString()
    {
        // Arrange
        string filePath = @"C:\StartMenu\MyApp\MyApp.lnk";
        string targetPath = @"C:\Programs\MyApp\MyApp.exe";

        // Act
        FileShortcut fileShortcut = FileShortcut.FromString($"{filePath};{targetPath}");

        // Assert
        Assert.Equal(filePath, fileShortcut.FilePath);
        Assert.Equal(targetPath, fileShortcut.TargetPath);
    }

    [Theory]
    [InlineData(@" ")]
    [InlineData(@";")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk;;C:\StartMenu\MyApp\MyApp.exe")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.srtct;C:\StartMenu\MyApp\MyApp.exe")]
    public void FromString_InvalidSyntax_ThrowArgumentException(string shortcutPathSyntax) =>

        // Act and assert
        Assert.Throws<ArgumentException>(nameof(shortcutPathSyntax), () => FileShortcut.FromString(shortcutPathSyntax));

    [Fact]
    public void FromString_NullSyntax_ThrowArgumentNullException() =>

        // Act and assert
        Assert.Throws<ArgumentNullException>("shortcutPathSyntax", () => FileShortcut.FromString(null!));

    [Theory]
    [InlineData(@".lnk;")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk;")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk;C:\StartMenu\MyApp\MyApp.exe")]
    public void FromString_ValidSyntax_ReturnsFileShortcut(string shortcutPathSyntax)
    {
        // Act
        FileShortcut fileShortcut = FileShortcut.FromString(shortcutPathSyntax);

        // Assert
        Assert.NotNull(fileShortcut);
    }

    [Fact]
    public void GetHashCode_ReturnsExpectedValue()
    {
        // Arrange
        string filePath = @"C:\StartMenu\MyApp\MyApp.lnk";
        string targetPath = @"C:\Programs\MyApp\MyApp.exe";
        string shortcutPathSyntax = $"{filePath};{targetPath}";
        FileShortcut fileShortcut = FileShortcut.FromString(shortcutPathSyntax);

        // Act
        int result = fileShortcut.GetHashCode();

        // Assert
        Assert.Equal(shortcutPathSyntax.GetHashCode(StringComparison.OrdinalIgnoreCase), result);
    }

    [Theory]
    [InlineData(@";")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk;;C:\StartMenu\MyApp\MyApp.exe")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.srtct;C:\StartMenu\MyApp\MyApp.exe")]
    public void TryConvertFrom_InvalidSyntax_ReturnsFalseAndNull(string shortcutPathSyntax)
    {
        // Act
        bool result = FileShortcut.TryConvertFrom(shortcutPathSyntax, out FileShortcut? fileShortcut);

        // Assert
        Assert.False(result);
        Assert.Null(fileShortcut);
    }

    [Theory]
    [InlineData(@".lnk;")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk;")]
    [InlineData(@"C:\StartMenu\MyApp\MyApp.lnk;C:\StartMenu\MyApp\MyApp.exe")]
    public void TryConvertFrom_ValidSyntax_ReturnsTrueAndNonNull(string shortcutPathSyntax)
    {
        // Act
        bool result = FileShortcut.TryConvertFrom(shortcutPathSyntax, out FileShortcut? fileShortcut);

        // Assert
        Assert.True(result);
        Assert.NotNull(fileShortcut);
    }
}
