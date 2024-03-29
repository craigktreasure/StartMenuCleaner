﻿namespace StartMenuCleaner.TestLibrary;

/// <summary>
/// Class TemporaryFile.
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public class TemporaryFile : IDisposable
{
    private bool disposedValue;

    /// <summary>
    /// Gets the file path.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporaryFile"/> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public TemporaryFile(string filePath)
    {
        this.FilePath = filePath;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                if (File.Exists(this.FilePath))
                {
                    File.Delete(this.FilePath);
                }
            }

            this.disposedValue = true;
        }
    }
}
