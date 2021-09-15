namespace StartMenuCleaner.TestLibrary;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

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
        if (resourceAssembly is null)
        {
            throw new ArgumentNullException(nameof(resourceAssembly));
        }

        if (fileSystem is null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        this.resourceAssembly = resourceAssembly;
        this.fileSystem = fileSystem;
    }

    /// <summary>
    /// Gets the embedded resource names.
    /// </summary>
    /// <returns><see cref="IReadOnlyList{System.String}"/>.</returns>
    public IReadOnlyList<string> GetResourceNames() => this.resourceAssembly.GetManifestResourceNames();

    /// <summary>
    /// Gets the embedded resource information.
    /// </summary>
    /// <param name="embeddedResourcePath">The embedded resource path.</param>
    /// <returns><see cref="ManifestResourceInfo"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public ManifestResourceInfo GetResourceInfo(string embeddedResourcePath)
    {
        ManifestResourceInfo? resourceInfo = this.resourceAssembly.GetManifestResourceInfo(embeddedResourcePath);

        if (resourceInfo is null)
        {
            throw new ArgumentException($"The embedded resource could not be found in {this.resourceAssembly.FullName}: '{embeddedResourcePath}'.", nameof(embeddedResourcePath));
        }

        return resourceInfo;
    }

    /// <summary>
    /// Loads the embedded resource  to a stream.
    /// </summary>
    /// <param name="embeddedResourcePath">The embedded resource path.</param>
    /// <returns><see cref="Stream"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public Stream LoadStream(string embeddedResourcePath)
    {
        Stream? embeddedResourceStream = this.resourceAssembly.GetManifestResourceStream(embeddedResourcePath);

        if (embeddedResourceStream is null)
        {
            throw new ArgumentException($"The embedded resource could not be found in {this.resourceAssembly.FullName}: '{embeddedResourcePath}'.", nameof(embeddedResourcePath));
        }

        return embeddedResourceStream;
    }

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
    /// <returns><see cref="System.String"/>.</returns>
    public string CopyToTempFile(string embeddedResourcePath)
    {
        string temporaryFileName = this.fileSystem.Path.GetTempFileName();

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
}
