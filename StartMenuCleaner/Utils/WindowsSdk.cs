namespace StartMenuCleaner.Utils;

using System;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;

internal static class WindowsSdk
{
    public static unsafe string ResolveShortcut(string shortcutFilePath)
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
        {
            throw new NotSupportedException($"{nameof(ResolveShortcut)} is only supported on Windows 5.1.2600+.");
        }

        // See the following issues for current status and new advice:
        // https://github.com/microsoft/CsWin32/issues/453
        // https://github.com/microsoft/CsWin32/discussions/323
        IPersistFile shellLink = (IPersistFile)(Activator.CreateInstance(Type.GetTypeFromCLSID(typeof(ShellLink).GUID, throwOnError: true)!)
            ?? throw new InvalidOperationException("Failed to create an instance of ShellLink"));

        fixed (char* shortcutFilePathPcwstr = shortcutFilePath)
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
