namespace StartMenuCleaner.Utils
{
    internal class DefaultFileShortcutHandler : IFileShortcutHandler
    {
        /// <summary>
        /// Resolves the target path of the specified shortcut.
        /// </summary>
        /// <param name="shortcutPath">The shortcut path.</param>
        /// <returns><see cref="System.String" />.</returns>
        public string ResolveTarget(string shortcutPath)
            => NativeMethods.ResolveShortcut(shortcutPath);
    }
}
