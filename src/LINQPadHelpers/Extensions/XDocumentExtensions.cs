using System.Xml.Linq;
using LINQPad;

namespace LINQPadHelpers.Extensions;

public static class XDocumentExtensions
{
    // I would have thought there'd be a type between xobject and xelement/xattribute that had the .Value property, but apparently not, only in legacy XmlNode 
    /// <summary>Get value of an xelement as a string, or the value specified if null.</summary>
    public static string GetValueOrDefault(this XElement? element, string defaultValue = "")
        => element?.Value?.ToString() ?? defaultValue;
    /// <summary>Get value of an xelement as T, or the value specified if not null, or the type conversion failed.</summary>
    public static T GetValueOrDefault<T>(this XElement? element, T defaultValue) where T : struct
        => element?.Value != null
               ? Util.Try(() => (T)Convert.ChangeType(element.Value, typeof(T)), defaultValue)
               : defaultValue;
    /// <summary>Get value of an xelement as T, or default(T) if: not found, or the type conversion failed.</summary>
    public static T GetValueOrDefault<T>(this XElement? element) where T : struct
        => GetValueOrDefault<T>(element, default(T));
    /// <summary>Get value of an xattribute as a string, or the value specified if null.</summary>
    public static string GetValueOrDefault(this XAttribute? attr, string defaultValue = "")
        => attr?.Value?.ToString() ?? defaultValue;
    /// <summary>Get value of an xattribute as T, or the value specified if not null, or the type conversion failed.</summary>
    public static T GetValueOrDefault<T>(this XAttribute? element, T defaultValue) where T : struct
        => element?.Value != null
               ? Util.Try(() => (T)Convert.ChangeType(element.Value, typeof(T)), defaultValue)
               : defaultValue;
    /// <summary>Get value of an xattribute as T, or default(T) if: not found, or the type conversion failed.</summary>
    public static T GetValueOrDefault<T>(this XAttribute? element) where T : struct
        => GetValueOrDefault<T>(element, default(T));
}