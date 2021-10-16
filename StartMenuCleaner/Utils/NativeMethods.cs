namespace StartMenuCleaner.Utils;

using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.UI.Shell;

/// <summary>
/// A helper class used to get the targets of .lnk files.
/// Significant code contribution from http://stackoverflow.com/questions/139010/how-to-resolve-a-lnk-in-c-sharp.
/// </summary>
internal static class NativeMethods
{
    #region Signatures imported from http://pinvoke.net

    /// <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
    private interface IShellLinkW
    {
        /// <summary>Retrieves the path and file name of a Shell link object</summary>
        void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);

        /// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
        void GetIDList(out IntPtr ppidl);

        /// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
        void SetIDList(IntPtr pidl);

        /// <summary>Retrieves the description string for a Shell link object</summary>
        void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

        /// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>Retrieves the name of the working directory for a Shell link object</summary>
        void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

        /// <summary>Sets the name of the working directory for a Shell link object</summary>
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        /// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
        void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

        /// <summary>Sets the command-line arguments for a Shell link object</summary>
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        /// <summary>Retrieves the hot key for a Shell link object</summary>
        void GetHotkey(out short pwHotkey);

        /// <summary>Sets a hot key for a Shell link object</summary>
        void SetHotkey(short wHotkey);

        /// <summary>Retrieves the show command for a Shell link object</summary>
        void GetShowCmd(out int piShowCmd);

        /// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
        void SetShowCmd(int iShowCmd);

        /// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
        void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

        /// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

        /// <summary>Sets the relative path to the Shell link object</summary>
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

        /// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed</summary>
        void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);

        /// <summary>Sets the path and file name of a Shell link object</summary>
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport, Guid("0000010c-0000-0000-c000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersist
    {
        [PreserveSig]
        void GetClassID(out Guid pClassID);
    }

    [ComImport, Guid("0000010b-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistFile : IPersist
    {
        new void GetClassID(out Guid pClassID);

        [PreserveSig]
        int IsDirty();

        [PreserveSig]
        void Load([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

        [PreserveSig]
        void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

        [PreserveSig]
        void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        [PreserveSig]
        void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
    }

    // CLSID_ShellLink from ShlGuid.h
    [
        ComImport(),
        Guid("00021401-0000-0000-C000-000000000046")
    ]
    internal class ShellLink
    {
    }

    #endregion Signatures imported from http://pinvoke.net

    public static string ResolveShortcut(string filename)
    {
        ShellLink link = new();
        ((IPersistFile)link).Load(filename, Constants.STGM_READ);
        // TODO: if I can get hold of the hwnd call resolve first. This handles moved and renamed files.
        // ((IShellLinkW)link).Resolve(hwnd, 0)
        StringBuilder sb = new((int)Constants.MAX_PATH);
        WIN32_FIND_DATAW data = new();
        ((IShellLinkW)link).GetPath(sb, sb.Capacity, out data, 0);
        return sb.ToString();
    }
}
