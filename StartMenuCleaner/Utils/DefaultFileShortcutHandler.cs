namespace StartMenuCleaner.Utils;

internal class DefaultFileShortcutHandler : IFileShortcutHandler
{
    /// <summary>
    /// Resolves the target path of the specified shortcut.
    /// </summary>
    /// <param name="shortcutPath">The shortcut path.</param>
    /// <returns><see cref="string" />.</returns>
    public string ResolveTarget(string shortcutPath)
        => WindowsSdk.ResolveShortcut(shortcutPath);
}
