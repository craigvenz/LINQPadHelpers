using System.IO.Compression;
using LINQPad;
using LINQPad.Uncapsulator;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LINQPadHelpers.Extensions;

public static class ConvenienceMethods
{
    #region Json.Net
    private static JsonSerializer? _cachedSerializer;

    /// <summary>Get default preferred serializer settings.</summary>
    public static JsonSerializerSettings CustomDefaultSettings
        => new()
           {
               ContractResolver = new OrderedContractResolver(),
               MissingMemberHandling = MissingMemberHandling.Ignore,
               Formatting = Formatting.Indented,
               ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
               NullValueHandling = NullValueHandling.Ignore,
               StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
               DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
               Converters = [new TolerantEnumConverter()]
           };
    /// <summary>Get default preferred serializer settings after making supplied changes.</summary>
    public static JsonSerializerSettings CustomizeDefaultSettings(Action<JsonSerializerSettings> customizer) 
    {
        var x = CustomDefaultSettings;
        customizer?.Invoke(x);
        return x;
    }
    /// <summary>Get JsonSerializer. Use my custom default settings above. If one's already been created return that.</summary>
    public static JsonSerializer CreateJsonSerializer()
        => _cachedSerializer ??= JsonSerializer.CreateDefault(CustomDefaultSettings);

    /// <summary>Used internally - sorts json properties in alphabetical order. Useful when doing diffs on json pieces.</summary>
    public class OrderedContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) =>
            [.. base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName)];
    }
    /// <summary>Read json from a zip stream of json.</summary>
    public static T? ReadJsonZip<T>(string filename)
    {
        using var f = File.OpenRead(filename);
        using var z = new DeflateStream(f, CompressionMode.Decompress);
        var serializer = CreateJsonSerializer();
        using var jw = new JsonTextReader(new StreamReader(z));
        return serializer.Deserialize<T>(jw);
    }
    /// <summary>Write json serialized version of object to disk as a zip stream.</summary>
    public static void WriteJsonZip(string filename, object data)
    {
        using var f = File.Create(filename);
        using var z = new DeflateStream(f, CompressionMode.Compress);
        var serializer = CreateJsonSerializer();
        using var jw = new JsonTextWriter(new StreamWriter(z));
        serializer.Serialize(jw, data);
    }
    #endregion
    /// <summary>Get desktop path</summary>
    public static string DesktopFolderPath(string filename) 
        => FolderPath(Environment.SpecialFolder.DesktopDirectory, filename);
    /// <summary>Get a special folder path</summary>
    public static string FolderPath(Environment.SpecialFolder folder, params string[] paths) 
        => Path.Combine(paths.Prepend(Environment.GetFolderPath(folder)).ToArray());
    /// <summary>Gets the 'shadow' folder where referenced dlls in the query are copied to.</summary>
    public static string GetExecutionShadowFolder() 
        => (string)Util.AssemblyLoadContext.Uncapsulate().ProbingSet.ShadowFolder.ToObject();
}