namespace StartMenuCleaner.Utils;

using System;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;

internal class CsWin32ShortcutHandler : FileShortcutHandler
{
    public override string ResolveTarget(string shortcutPath)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
        {
            throw new NotSupportedException($"{nameof(ResolveTarget)} is only supported on Windows 5.1.2600+.");
        }

        IPersistFile shellLink = (IPersistFile)new ShellLink();
        shellLink.Load(shortcutPath, STGM.STGM_READ);

        Span<char> szShortcutTargetPath = stackalloc char[(int)PInvoke.MAX_PATH];
        IShellLinkW shellLinkW = (IShellLinkW)shellLink;
        WIN32_FIND_DATAW data = new();
        shellLinkW.GetPath(szShortcutTargetPath, ref data, 0);

        Marshal.ReleaseComObject(shellLinkW);
        Marshal.ReleaseComObject(shellLink);

        return szShortcutTargetPath[..szShortcutTargetPath.IndexOf('\0')].ToString();
    }
}
