using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using LPControls = LINQPad.Controls;

namespace LINQPadHelpers.PInvoke;

public partial class Icons
{
    public static LPControls.Image GetImageForExtension(string extension)
    {
        try
        {
            return new(IconFromExtension(extension, SystemIconSize.Small));
        }
        catch
        {
            return new(IconFromExtension(".", SystemIconSize.Small));
        }
    }

    internal static Bitmap? IconFromExtension(string extension, SystemIconSize size)
    {
        if (!extension.StartsWith('.'))
            extension = '.' + extension;

        Shell.AssocCreate(Shell.CLSID_QueryAssociations, ref Shell.IID_IQueryAssociations, out var obj);
        var qa = (Shell.IQueryAssociations)obj;
        qa.Init(Shell.ASSOCF.INIT_DEFAULTTOSTAR, extension, UIntPtr.Zero, IntPtr.Zero);

        var bufSize = 0;
        qa.GetString(Shell.ASSOCF.NOTRUNCATE, Shell.ASSOCSTR.DEFAULTICON, null, null, ref bufSize);

        var sb = new StringBuilder(bufSize);
        qa.GetString(Shell.ASSOCF.NOTRUNCATE, Shell.ASSOCSTR.DEFAULTICON, null, sb, ref bufSize);

        if (string.IsNullOrEmpty(sb.ToString())) 
            return null;
        var iconLocation = sb.ToString();
        var iconPath = iconLocation.Split(',');

        var iIconPathNumber = iconPath.Length > 1 ? 1 : 0;

        var large = new IntPtr[1];
        var small = new IntPtr[1];

        //extracts the icon from the file.
        var hResult = ExtractIconEx(iconPath[0],
                                    iIconPathNumber > 0 ? Convert.ToInt16(iconPath[iIconPathNumber]) : Convert.ToInt16(0),
                                    large,
                                    small, 1);
        if (hResult != 0)
        {
            throw new Exception("Failed to extract icon from file.");
        }
        //(large,small).Dump();
        var handle = size == SystemIconSize.Large ? large[0] : small[0];
        return Bitmap.FromHicon(handle);
    }

    internal enum SystemIconSize : int
    {
        Large = 0x000000000,
        Small = 0x000000001
    }

    [LibraryImport("Shell32", StringMarshalling = StringMarshalling.Utf8)]
    private static partial int ExtractIconEx(
        [MarshalAs(UnmanagedType.LPTStr)] string lpszFile,
        int nIconIndex,
        [Out]IntPtr[] phIconLarge,
        [Out]IntPtr[] phIconSmall,
        int nIcons);

    internal partial class Shell
    {
        [LibraryImport("shlwapi.dll")]
        public static partial int AssocCreate(
            Guid clsid,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);

        [Flags]
        public enum ASSOCF
        {
            INIT_NOREMAPCLSID = 0x00000001,
            INIT_BYEXENAME = 0x00000002,
            OPEN_BYEXENAME = 0x00000002,
            INIT_DEFAULTTOSTAR = 0x00000004,
            INIT_DEFAULTTOFOLDER = 0x00000008,
            NOUSERSETTINGS = 0x00000010,
            NOTRUNCATE = 0x00000020,
            VERIFY = 0x00000040,
            REMAPRUNDLL = 0x00000080,
            NOFIXUPS = 0x00000100,
            IGNOREBASECLASS = 0x00000200,
            INIT_IGNOREUNKNOWN = 0x00000400
        }

        public enum ASSOCSTR
        {
            COMMAND = 1,
            EXECUTABLE,
            FRIENDLYDOCNAME,
            FRIENDLYAPPNAME,
            NOOPEN,
            SHELLNEWVALUE,
            DDECOMMAND,
            DDEIFEXEC,
            DDEAPPLICATION,
            DDETOPIC,
            INFOTIP,
            QUICKTIP,
            TILEINFO,
            CONTENTTYPE,
            DEFAULTICON,
            SHELLEXTENSION
        }

        public enum ASSOCKEY
        {
            SHELLEXECCLASS = 1,
            APP,
            CLASS,
            BASECLASS
        }

        public enum ASSOCDATA
        {
            MSIDESCRIPTOR = 1,
            NOACTIVATEHANDLER,
            QUERYCLASSSTORE,
            HASPERUSERASSOC,
            EDITFLAGS,
            VALUE
        }

        [Guid("c46ca590-3c3f-11d2-bee6-0000f805ca57")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IQueryAssociations
        {
            void Init(
                [In] ASSOCF flags,
                [In][MarshalAs(UnmanagedType.LPWStr)] string pszAssoc,
                [In] UIntPtr hkProgid,
                [In] IntPtr hwnd);

            void GetString(
                [In] ASSOCF flags,
                [In] ASSOCSTR str,
                [In][MarshalAs(UnmanagedType.LPWStr)] string? pwszExtra,
                [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder? pwszOut,
                [In][Out] ref int pcchOut);

            void GetKey(
                [In] ASSOCF flags,
                [In] ASSOCKEY str,
                [In][MarshalAs(UnmanagedType.LPWStr)] string pwszExtra,
                [Out] out UIntPtr phkeyOut);

            void GetData(
                [In] ASSOCF flags,
                [In] ASSOCDATA data,
                [In][MarshalAs(UnmanagedType.LPWStr)] string pwszExtra,
                [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
                out byte[] pvOut,
                [In][Out] ref int pcbOut);

            void GetEnum(); // not used actually
        }

        internal static Guid CLSID_QueryAssociations = new("a07034fd-6caa-4954-ac3f-97a27216f98a");
        internal static Guid IID_IQueryAssociations = new("c46ca590-3c3f-11d2-bee6-0000f805ca57");
    }
}