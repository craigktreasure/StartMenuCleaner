namespace StartMenuCleaner.Utils;

using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;

internal static class WindowsSdk
{
    // CLSID_ShellLink from ShlGuid.h
    [
        ComImport(),
        Guid("00021401-0000-0000-C000-000000000046")
    ]
    internal class ShellLink
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility",
        Justification = "PackAsTool doesn't support targeting specific platforms.")]
    public static unsafe string ResolveShortcut(string shortcutFilePath)
    {
        ShellLink shellLink = new();

        IPersistFile persistFile = (IPersistFile)shellLink;
        fixed (char* shortcutFilePathPcwstr = shortcutFilePath)
        {
            persistFile.Load(shortcutFilePathPcwstr, PInvoke.STGM_READ);
        }

        Span<char> szShortcutTargetPath = stackalloc char[(int)PInvoke.MAX_PATH];
        fixed (char* cShortcutTargetPath = szShortcutTargetPath)
        {
            IShellLinkW shellLinkW = (IShellLinkW)shellLink;
            WIN32_FIND_DATAW data;
            shellLinkW.GetPath(cShortcutTargetPath, (int)PInvoke.MAX_PATH, &data, 0);

            return new string(cShortcutTargetPath);
        }
    }
}
