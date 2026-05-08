using LINQPad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LINQPadHelpers.Extensions;

public static class NewtonsoftJsonExtensions
{
    /// <summary>Call dump on the object after converting to json</summary>
    public static object? DumpAsJson(this object? obj, string name = "")
    {
        if (obj == null)
            return "{}".Dump(name);
        return Util.SyntaxColorText(obj.ToJson(), SyntaxLanguageStyle.Json, true).Dump(name);
    }
    /// <summary>Convert this object to a json string</summary>
    public static string ToJson(this object obj)
    {
        using var sw = new StringWriter();
        ConvenienceMethods.CreateJsonSerializer().Serialize(sw, obj);
        return sw.GetStringBuilder().ToString();
    }

    /// <summary>JsonReader from a stream</summary>
    public static JsonTextReader GetJsonReader(this Stream f) => new(new StreamReader(f));
    /// <summary>JsonReader from a TextReader</summary>
    public static JsonTextReader GetJsonReader(this TextReader r) => new(r);
    /// <summary>JsonReader from a string</summary>
    public static JsonTextReader GetJsonReader(this string s) => new StringReader(s).GetJsonReader();
    /// <summary>Deserialize json string to object</summary>
    public static T? DeserializeJsonString<T>(this string input, JsonSerializerSettings? settings = null) =>
        JsonConvert.DeserializeObject<T>(input, settings ?? ConvenienceMethods.CustomDefaultSettings);
    /// <summary>Read json from a file given as the string</summary>    
    public static T? DeserializeJsonFile<T>(this string fileName, JsonSerializerSettings? settings = null) =>
        (settings == null ? ConvenienceMethods.CreateJsonSerializer() : JsonSerializer.Create(settings))
        .Deserialize<T>(File.OpenRead(fileName).GetJsonReader());
    /// <summary>Write object as json to a file</summary>						  
    public static object WriteJsonFile(this object data, string fileName, JsonSerializerSettings? settings = null)
    {
        var set = settings == null ? ConvenienceMethods.CreateJsonSerializer() : JsonSerializer.Create(settings);
        using var f = File.Create(fileName);
        using var jw = new JsonTextWriter(new StreamWriter(f));
        set.Serialize(jw, data);
        return data;
    }

    /// <summary>Return date as json format, local time</summary>
    public static string ToJsonDate(this DateTime date) => date.ToString("yyyy-MM-ddTHH:mm:ssK");
    /// <summary>Return date as json format, shifted to UTC</summary>
    public static string ToUtcJsonDate(this DateTime date) => date.ToUniversalTime().ToJsonDate();
    /// <summary>Return date as json format, pretending it was originally UTC, no shifting done</summary>    
    public static string AsUtcJsonDate(this DateTime date) => new DateTime(date.Ticks, DateTimeKind.Utc).ToJsonDate();
    /// <summary>Retrieve a property from a JObject, cast to T automatically, or return null.</summary>
    public static T? Property<T>(this JObject? objects, string name)
    {
        var obj = objects?.Property(name);
        if (obj == null || obj.Type == JTokenType.None || obj.Type == JTokenType.Null)
            return default;
        return (T)Convert.ChangeType(obj.Value, typeof(T));
    }
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content) =>
        JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
    public static async Task<dynamic?> ReadAsJsonAsync(this HttpContent content) =>
        JsonConvert.DeserializeObject(await content.ReadAsStringAsync());
}