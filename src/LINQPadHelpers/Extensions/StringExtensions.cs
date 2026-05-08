using System.Text;
using System.Text.RegularExpressions;

namespace LINQPadHelpers.Extensions;

public static partial class StringExtensions
{
    /// <summary>If the string is not empty, return a transformation of it.</summary>
    public static string OnNotEmptyString(this string input, Func<string, string> func) => input.GetValueOrDefault(func(input));
    /// <summary>Return an alternative default value if the string is empty.</summary>
    public static string GetValueOrDefault(this string? input, string defaultValue = "") => !string.IsNullOrEmpty(input) ? input : defaultValue;
    /// <summary>Return the first string in a collection of strings that's not empty.</summary>
    public static string Coalesce(this IEnumerable<string?>? values) => (values ?? []).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? string.Empty;
    /// <summary>Return the first string in the parameter list that's not empty.</summary>
    public static string Coalesce(this string first, params string[]? rest) => Coalesce(new[] { first }.Concat(rest ?? []));
    /// <summary>Return first num of characters of string, and append ... if it's longer than that.</summary>
    public static string Truncate(this string? str, int num) => str != null ? $"{str[..num]}..." : str ?? string.Empty;
    /// <summary>Join a list of strings that match the condition.</summary>
    public static string JoinWhere(this IEnumerable<string> strings, string separator, Func<string, bool> predicate) => string.Join(separator, strings.Where(predicate));
    /// <summary>Join a list of only non-empty strings.</summary>
    public static string JoinWhereNotEmpty(this IEnumerable<string> strings, string separator = ", ") => JoinWhere(strings, separator, x => !string.IsNullOrWhiteSpace(x));
    /// <summary>Iso Date string type expected in Sitecore.</summary>
    public static string ToIsoString(this DateTime date) => date.ToString("yyyyMMddTHHmmss");
    private static readonly char[] Separators = [',', '\n', '|'];
    /// <summary>Split a string into a list of Guids.</summary>
    public static List<Guid> ToGuidList(this string str) =>
        str.Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
           .Select(x => Guid.TryParse(x, out var g) ? (Guid?)g : null)
           .Where(x => x.HasValue)
           .Cast<Guid>()
           .Distinct()
           .ToList();
#if NET5 && !NET6 && !NET7 && !NET8_0
// This is standard in System.Linq as of .net 6
    /// <summary>MKRs chunk up a list of data into size chunks.</summary>
    public static List<List<T>> Chunk<T>(this IEnumerable<T> data, int size)
    {
        return data.Select((x, i) => new { Index = i, Value = x })
                   .GroupBy(x => x.Index / size)
                   .Select(x => x.Select(v => v.Value).ToList())
                   .ToList();
    }
#endif
    /// <summary>Debug print representation of a collection.</summary>
    public static string ToPrintableList<T>(this IEnumerable<T>? list, string d = ", ") =>
        $"IEnumerable<{typeof(T).Name}> {(list == null ? "(null)" : list.Any() ? $"[{string.Join(d, list)}]" : "[]")}";
    /// <summary>Return a hexadecimal string representation of a byte array.</summary>
    public static string ToHexString(this byte[] data) => BitConverter.ToString(data).Replace("-", string.Empty);
    /// <summary>Compute a hash for a string.</summary>
    /// <param name="data">The string to compute the hash for.</param>
    /// <param name="decoderFunc">Supply an optional text decoder - defaults to UTF8.</param>
    /// <param name="hashFunc">Supply an optional hash function - defaults to SHA256.</param>
    public static string ComputeHash(this string data, Func<string, byte[]>? decoderFunc = null, Func<byte[], byte[]>? hashFunc = null)
        => (hashFunc ?? System.Security.Cryptography.SHA256.HashData)((decoderFunc ?? System.Text.Encoding.UTF8.GetBytes)(data)).ToHexString();
    /// <summary>Compute hash from a stream.</summary>
    /// <param name="stream">Stream containing the data to hash.</param>
    /// <param name="hashFunc">An optionally provided hash function. Defaults to SHA256.</param>
    /// <remarks>Streams passed to this function will be disposed on completion.</remarks>
    public static string ComputeHash(this Stream stream, Func<Stream, byte[]>? hashFunc = null) => stream.Using(s => (hashFunc ?? System.Security.Cryptography.SHA256.HashData)(s).ToHexString()) ?? string.Empty;
    /// <summary>LCPTracker's CleanFileName function.</summary>
    public static string CleanFileName(this string fileName) =>
        //Replace all special characters with a dash (-)
        CleanFileNameRegex().Replace(fileName, "-").Replace("\t", "-").Trim();

    [GeneratedRegex(@"[/&@#%()*:!$\^+={}|\\"";'<>?\[\]]")]
    private static partial Regex CleanFileNameRegex();
}
