namespace StartMenuCleaner.Utils;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// A utility class for handling file shortcuts.
/// A shortcut path syntax can be used to convert to a <see cref="FileShortcut"/>: "{file_path};{target_path}".
/// Implements the <see cref="FileShortcut" />
/// </summary>
public class FileShortcut : IEquatable<FileShortcut>
{
    private const char fragmentSeparator = ';';

    private const string lnkFileExtension = ".lnk";

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
    /// Determines whether the specified value contains shortcut path syntax ("{file_path};{target_path}").
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>True if the value contains shortcut path syntax; otherwise false.</returns>
    public static bool ContainsShortcutPathSyntax(string value)
    {
        return value is not null
            && value.Count(x => x == fragmentSeparator) == 1
            && value.Contains($"{lnkFileExtension}{fragmentSeparator}", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="string"/> to <see cref="FileShortcut"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator FileShortcut(string value) => FromString(value);

    /// <summary>
    /// Performs an explicit conversion from <see cref="FileShortcut"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="fileShortcut">The shortcut path.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(FileShortcut fileShortcut) => Argument.NotNull(fileShortcut).ToString();

    /// <summary>
    /// Converts shortcut path syntax ("{file_path};{target_path}") to a <see cref="FileShortcut"/>.
    /// </summary>
    /// <param name="shortcutPathSyntax">The shortcut path syntax.</param>
    /// <returns><see cref="FileShortcut"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static FileShortcut FromString(string shortcutPathSyntax)
    {
        if (shortcutPathSyntax is null)
        {
            throw new ArgumentNullException(nameof(shortcutPathSyntax));
        }

        if (!ContainsShortcutPathSyntax(shortcutPathSyntax))
        {
            throw new ArgumentException($"Value specified does not contain shortcut path syntax: {shortcutPathSyntax}.", nameof(shortcutPathSyntax));
        }

        string[] fragments = shortcutPathSyntax.Split(fragmentSeparator, StringSplitOptions.TrimEntries);

        if (fragments.Length is not 2)
        {
            throw new ArgumentException("Shortcut path is not propertly formatted.", nameof(shortcutPathSyntax));
        }

        string path = fragments[0];
        string target = fragments[1];

        if (!Path.GetExtension(path).Equals(lnkFileExtension, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Shortcut path does not contain a link (.lnk) file.", nameof(shortcutPathSyntax));
        }

        return new FileShortcut(path, target);
    }

    /// <summary>
    /// Tries to convert shortcut path syntax ("{file_path};{target_path}") to a <see cref="FileShortcut"/>.
    /// </summary>
    /// <param name="shortcutPathSyntax">The shortcut path syntax.</param>
    /// <param name="fileShortcut">The shortcut path.</param>
    /// <returns><see cref="bool"/>.</returns>
    public static bool TryConvertFrom(string shortcutPathSyntax, [NotNullWhen(true)] out FileShortcut? fileShortcut)
    {
        Argument.NotNullOrWhiteSpace(shortcutPathSyntax);

        fileShortcut = null;

        if (!ContainsShortcutPathSyntax(shortcutPathSyntax))
        {
            return false;
        }

        string[] fragments = shortcutPathSyntax.Split(fragmentSeparator, StringSplitOptions.TrimEntries);

        if (fragments.Length is not 2)
        {
            return false;
        }

        string path = fragments[0];
        string target = fragments[1];

        if (!Path.GetExtension(path).Equals(lnkFileExtension, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        fileShortcut = new FileShortcut(path, target);
        return true;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => this.Equals(obj as FileShortcut);

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
        (this.ToString()).GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Converts to a string using shortcut path syntax: "{file_path};{target_path}".
    /// </summary>
    /// <returns><see cref="string"/>.</returns>
    public override string ToString() =>
        $"{this.FilePath}{fragmentSeparator}{this.TargetPath}";
}
