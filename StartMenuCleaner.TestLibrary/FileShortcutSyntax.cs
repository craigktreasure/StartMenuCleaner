namespace StartMenuCleaner.TestLibrary
{
    using StartMenuCleaner.Utils;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// A utility class for handling shortcut path syntax: "<file_path>;<target_path>".
    /// Implements the <see cref="StartMenuCleaner.Utils.FileShortcut" />
    /// </summary>
    /// <seealso cref="StartMenuCleaner.Utils.FileShortcut" />
    public class FileShortcutSyntax : FileShortcut
    {
        private const char fragmentSeparator = ';';

        private const string lnkFileExtension = ".lnk";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShortcutSyntax"/> class.
        /// </summary>
        /// <param name="filePath">The shortcut file path.</param>
        /// <param name="targetPath">The shortcut target path.</param>
        public FileShortcutSyntax(string filePath, string targetPath)
            : base(filePath, targetPath) { }

        /// <summary>
        /// Determines whether the specified value contains shortcut path syntax.
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
        /// Converts shortcut path syntax to a <see cref="FileShortcutSyntax"/>.
        /// </summary>
        /// <param name="shortcutPathSyntax">The shortcut path syntax.</param>
        /// <returns><see cref="StartMenuCleaner.TestLibrary.FileShortcutSyntax"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static FileShortcutSyntax ConvertFrom(string shortcutPathSyntax)
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

            if (!System.IO.Path.GetExtension(path).Equals(lnkFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Shortcut path does not contain a link (.lnk) file.", nameof(shortcutPathSyntax));
            }

            return new FileShortcutSyntax(path, target);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="string"/> to <see cref="StartMenuCleaner.TestLibrary.FileShortcutSyntax"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator FileShortcutSyntax(string value) => ConvertFrom(value);

        /// <summary>
        /// Performs an explicit conversion from <see cref="StartMenuCleaner.TestLibrary.FileShortcutSyntax"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="fileShortcut">The shortcut path.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator string(FileShortcutSyntax fileShortcut) => fileShortcut.ToString();

        /// <summary>
        /// Tries to convert shortcut path syntax to a <see cref="FileShortcutSyntax"/>.
        /// </summary>
        /// <param name="shortcutPathSyntax">The shortcut path syntax.</param>
        /// <param name="fileShortcut">The shortcut path.</param>
        /// <returns><see cref="bool"/>.</returns>
        public static bool TryConvertFrom(string shortcutPathSyntax, [NotNullWhen(true)] out FileShortcutSyntax? fileShortcut)
        {
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

            if (!System.IO.Path.GetExtension(path).Equals(lnkFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            fileShortcut = new FileShortcutSyntax(path, target);
            return true;
        }

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <returns><see cref="string"/>.</returns>
        public override string ToString() =>
            $"{this.FilePath}{fragmentSeparator}{this.TargetPath}";
    }
}
