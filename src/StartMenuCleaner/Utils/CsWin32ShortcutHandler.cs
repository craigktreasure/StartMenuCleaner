namespace StartMenuCleaner.Utils;

using System;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;

internal class CsWin32ShortcutHandler : IFileShortcutHandler
{
    public unsafe string ResolveTarget(string shortcutPath)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
        {
            throw new NotSupportedException($"{nameof(ResolveTarget)} is only supported on Windows 5.1.2600+.");
        }

        // See the following issues for current status and new advice:
        // https://github.com/microsoft/CsWin32/issues/453
        // https://github.com/microsoft/CsWin32/discussions/323
        IPersistFile shellLink = (IPersistFile)(Activator.CreateInstance(Type.GetTypeFromCLSID(typeof(ShellLink).GUID, throwOnError: true)!)
            ?? throw new InvalidOperationException("Failed to create an instance of ShellLink"));

        fixed (char* shortcutFilePathPcwstr = shortcutPath)
        {
            shellLink.Load(shortcutFilePathPcwstr, PInvoke.STGM_READ);
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