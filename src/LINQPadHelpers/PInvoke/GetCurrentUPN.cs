using System.Runtime.InteropServices;
using System.Text;

namespace LINQPadHelpers.PInvoke;

public static class GetCurrentUsername
{
    public static string? GetCurrentUPN()
    {
        var userUpn = new StringBuilder(1024);
        var userUpnSize = userUpn.Capacity;

        return GetUserNameEx((int)ExtendedFormat.NameUserPrincipal, userUpn, ref userUpnSize) != 0
                   ? userUpn.ToString()
                   : null;
    }

    [DllImport("secur32.dll", CharSet = CharSet.Unicode)]
    internal static extern int GetUserNameEx(int nameFormat, StringBuilder userName, ref int userNameSize);
}
// https://stackoverflow.com/a/67922270/223942
internal enum ExtendedFormat
{
    NameUnknown = 0,
    NameFullyQualifiedDN = 1,
    NameSamCompatible = 2,
    NameDisplay = 3,
    NameUniqueId = 6,
    NameCanonical = 7,
    NameUserPrincipal = 8,
    NameCanonicalEx = 9,
    NameServicePrincipal = 10,
    NameDnsDomain = 12,
}
