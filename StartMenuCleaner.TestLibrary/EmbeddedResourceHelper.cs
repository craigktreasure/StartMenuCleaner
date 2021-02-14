namespace StartMenuCleaner.TestLibrary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;
    using System.Reflection;

    public class EmbeddedResourceHelper
    {
        private readonly IFileSystem fileSystem;

        private readonly Assembly resourceAssembly;

        public EmbeddedResourceHelper(Assembly resourceAssembly)
            : this(resourceAssembly, new FileSystem()) { }

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

        public IReadOnlyList<string> GetResourceNames() => this.resourceAssembly.GetManifestResourceNames();

        public ManifestResourceInfo GetResourceInfo(string embeddedResourcePath)
        {
            ManifestResourceInfo? resourceInfo = this.resourceAssembly.GetManifestResourceInfo(embeddedResourcePath);

            if (resourceInfo is null)
            {
                throw new ArgumentException($"The embedded resource could not be found in {this.resourceAssembly.FullName}: '{embeddedResourcePath}'.", nameof(embeddedResourcePath));
            }

            return resourceInfo;
        }

        public Stream LoadStream(string embeddedResourcePath)
        {
            Stream? embeddedResourceStream = this.resourceAssembly.GetManifestResourceStream(embeddedResourcePath);

            if (embeddedResourceStream is null)
            {
                throw new ArgumentException($"The embedded resource could not be found in {this.resourceAssembly.FullName}: '{embeddedResourcePath}'.", nameof(embeddedResourcePath));
            }

            return embeddedResourceStream;
        }

        public void CopyToFileSystem(string embeddedResourcePath, string fileSystemPath)
        {
            using Stream embeddedResourceStream = this.LoadStream(embeddedResourcePath);
            using Stream newFileStream = this.fileSystem.File.OpenWrite(fileSystemPath);
            embeddedResourceStream.CopyTo(newFileStream);
        }

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
}
