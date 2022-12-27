namespace StartMenuCleaner;

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO.Abstractions;

internal class FileSystemOperationHandler
{
    private readonly IFileSystem fileSystem;

    private readonly ILogger<FileSystemOperationHandler> logger;

    private readonly bool simulate;

    public FileSystemOperationHandler(ILogger<FileSystemOperationHandler> logger, IFileSystem fileSystem, CleanerOptions options)
    {
        this.fileSystem = fileSystem;
        this.logger = logger;
        this.simulate = options.Simulate;
    }

    public void DeleteDirectory(string directoryPath, bool recurse = false)
    {
        if (!this.simulate)
        {
            this.fileSystem.Directory.Delete(directoryPath, recurse);
        }

        this.logger.DirectoryDeleted(this.fileSystem.Path.GetFileName(directoryPath));
    }

    public void DeleteFile(string filePath)
    {
        if (!this.fileSystem.File.Exists(filePath))
        {
            return;
        }

        if (!this.simulate)
        {
            this.fileSystem.File.Delete(filePath);
        }

        this.logger.FileDeleted(this.fileSystem.Path.GetFileName(filePath));
    }

    public void DeleteFiles(IEnumerable<string> filePaths)
    {
        foreach (string filePath in filePaths)
        {
            this.DeleteFile(filePath);
        }
    }

    public void MoveFilesToDirectory(string newDirectory, IEnumerable<string> currentFileLocations, bool replaceExisting = false)
    {
        foreach (string currentFileLocation in currentFileLocations)
        {
            this.MoveFileToDirectory(newDirectory, currentFileLocation, replaceExisting);
        }
    }

    public void MoveFileToDirectory(string newDirectory, string currentFileLocation, bool replaceExisting = false)
    {
        string newFileLocation = this.fileSystem.Path.Combine(newDirectory, this.fileSystem.Path.GetFileName(currentFileLocation));
        if (!this.simulate)
        {
            if (replaceExisting && this.fileSystem.File.Exists(newFileLocation))
            {
                this.DeleteFile(newFileLocation);
            }

            this.fileSystem.File.Move(currentFileLocation, newFileLocation);
        }

        this.logger.FileMoved(this.fileSystem.Path.GetFileName(currentFileLocation), newFileLocation);
    }
}
