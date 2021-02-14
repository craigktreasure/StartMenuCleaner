namespace StartMenuCleaner.Utils
{
    using System;

    public class FileShortcut : IEquatable<FileShortcut>
    {
        /// <summary>
        /// Gets the path to the shortcut file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the target path the shortcut points to.
        /// </summary>
        public string TargetPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcut"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="targetPath">The target path.</param>
        public FileShortcut(string filePath, string targetPath)
        {
            this.FilePath = filePath;
            this.TargetPath = targetPath;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as FileShortcut);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(FileShortcut? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.FilePath.Equals(other.FilePath, StringComparison.OrdinalIgnoreCase)
                && this.TargetPath.Equals(other.TargetPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() =>
            (this.FilePath + this.TargetPath).GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}
