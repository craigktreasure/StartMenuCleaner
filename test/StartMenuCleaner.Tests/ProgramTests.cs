namespace StartMenuCleaner.Tests;

using System.IO.Abstractions.TestingHelpers;

public sealed class ProgramTests
{
    [Fact]
    public void Run()
    {
        // Arrange
        ProgramOptions options = new();
        MockFileSystem fileSystem = new();

        // Act
        Program.Run(options, fileSystem);
    }

    [Fact]
    public void Run_WithDebugEnabled()
    {
        // Arrange
        ProgramOptions options = new()
        {
            Debug = true,
        };
        MockFileSystem fileSystem = new();

        // Act
        Program.Run(options, fileSystem);
    }

    [Fact]
    public void Run_WithSimulateEnabled()
    {
        // Arrange
        ProgramOptions options = new()
        {
            Simulate = true,
        };
        MockFileSystem fileSystem = new();

        // Act
        Program.Run(options, fileSystem);
    }
}
