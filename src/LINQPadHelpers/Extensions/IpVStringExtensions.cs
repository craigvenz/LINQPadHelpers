using System.Text.RegularExpressions;

namespace LINQPadHelpers.Extensions;

public static partial class IpVStringExtensions
{
    private static readonly Regex IpV4REgex = IpV4Regex();
    private static readonly Regex IpV6REgex = IpV6Regex();
    /// <summary>Does this string represent an IPv4 address?</summary>
    public static bool RegexIPv4(this string value) => !value.Contains(',') && IpV4REgex.IsMatch(value);
    /// <summary>Does this string represent an IPv6 address?</summary>
    public static bool RegexIPv6(this string value) => !value.Contains(',') && IpV6REgex.IsMatch(value);
    [GeneratedRegex(@"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})$", RegexOptions.Compiled)]
    private static partial Regex IpV4Regex();
    [GeneratedRegex(@"^((:?[0-9A-Fa-f]{0,4}\.?[0-9A-Fa-f]{0,4}){1,8})?", RegexOptions.Compiled)]
    private static partial Regex IpV6Regex();
}
