namespace StartMenuCleaner.TestLibrary;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

using StartMenuCleaner.TestLibrary.Extensions;

/// <summary>
/// Class EmbeddedResourceHelper.
/// </summary>
public class EmbeddedResourceHelper
{
    private readonly IFileSystem fileSystem;

    private readonly Assembly resourceAssembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceHelper"/> class.
    /// </summary>
    /// <param name="resourceAssembly">The resource assembly.</param>
    public EmbeddedResourceHelper(Assembly resourceAssembly)
        : this(resourceAssembly, new FileSystem()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceHelper"/> class.
    /// </summary>
    /// <param name="resourceAssembly">The resource assembly.</param>
    /// <param name="fileSystem">The file system.</param>
    /// <exception cref="ArgumentNullException">
    /// resourceAssembly
    /// or
    /// fileSystem
    /// </exception>
    public EmbeddedResourceHelper(Assembly resourceAssembly, IFileSystem fileSystem)
    {
        ArgumentNullException.ThrowIfNull(resourceAssembly);
        ArgumentNullException.ThrowIfNull(fileSystem);

        this.resourceAssembly = resourceAssembly;
        this.fileSystem = fileSystem;
    }

    /// <summary>
    /// Gets the embedded resource names.
    /// </summary>
    /// <returns>An <see cref="IReadOnlyList{T}"/> of <see cref="string"/>.</returns>
    public IReadOnlyList<string> GetResourceNames() => this.resourceAssembly.GetManifestResourceNames();

    /// <summary>
    /// Gets the embedded resource information.
    /// </summary>
    /// <param name="embeddedResourcePath">The embedded resource path.</param>
    /// <returns><see cref="ManifestResourceInfo"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public ManifestResourceInfo GetResourceInfo(string embeddedResourcePath)
        => this.resourceAssembly.GetManifestResourceInfo(embeddedResourcePath)
            ?? throw new ArgumentException($"The embedded resource could not be found in {this.resourceAssembly.FullName}: '{embeddedResourcePath}'.", nameof(embeddedResourcePath));

    /// <summary>
    /// Loads the embedded resource  to a stream.
    /// </summary>
    /// <param name="embeddedResourcePath">The embedded resource path.</param>
    /// <returns><see cref="Stream"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Stream LoadStream(string embeddedResourcePath)
        => this.resourceAssembly.GetManifestResourceStream(embeddedResourcePath)
            ?? throw new ArgumentException($"The embedded resource could not be found in {this.resourceAssembly.FullName}: '{embeddedResourcePath}'.", nameof(embeddedResourcePath));

    /// <summary>
    /// Copies the embedded resource to the file system.
    /// </summary>
    /// <param name="embeddedResourcePath">The embedded resource path.</param>
    /// <param name="fileSystemPath">The file system path.</param>
    public void CopyToFileSystem(string embeddedResourcePath, string fileSystemPath)
    {
        using Stream embeddedResourceStream = this.LoadStream(embeddedResourcePath);
        using Stream newFileStream = this.fileSystem.File.OpenWrite(fileSystemPath);
        embeddedResourceStream.CopyTo(newFileStream);
    }

    /// <summary>
    /// Copies the embedded resource to a temporary file.
    /// </summary>
    /// <param name="embeddedResourcePath">The embedded resource path.</param>
    /// <returns><see cref="string"/>.</returns>
    public string CopyToTempFile(string embeddedResourcePath)
    {
        string temporaryFileName = this.fileSystem.Path.GetTemporaryFilePath();

        try
        {
            this.CopyToFileSystem(embeddedResourcePath, temporaryFileName);
        }
        catch
        {
            this.fileSystem.File.Delete(temporaryFileName);
            throw;
        }

        return temporaryFileName;
    }

    /// <summary>
    /// Gets the specified embedded resource as a <see cref="TemporaryFile" />.
    /// </summary>
    /// <param name="embeddedResourcePath">The embedded resource path.</param>
    /// <returns><see cref="TemporaryFile"/>.</returns>
    public TemporaryFile ToTemporaryFile(string embeddedResourcePath)
    {
        string temporaryFilePath = this.CopyToTempFile(embeddedResourcePath);

        return new TemporaryFile(temporaryFilePath);
    }
}
