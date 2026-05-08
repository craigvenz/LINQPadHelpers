using System.Text.RegularExpressions;
using LINQPad;

namespace LINQPadHelpers.Extensions;

public static class RegexExtensions
{
    /// <summary>Return all Regex matches in the string of the pattern supplied.</summary>
    public static IEnumerable<Match> RegEx(this string inVal, string pattern, RegexOptions options = RegexOptions.None)
        => Regex.Matches(inVal, pattern, options);
   
    /// <summary>Return the number of Regex matches in the string of the pattern supplied.</summary>
    public static int RegExCount(this string inVal, string pattern, RegexOptions options = RegexOptions.None)
        => Util.Try( () => Regex.Matches(inVal, pattern, options).Count,
                    _ => 0, true);
    
    public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options = RegexOptions.None)
        => Regex.Replace(input, pattern, replacement, options);
}