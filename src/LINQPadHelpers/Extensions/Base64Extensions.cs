namespace LINQPadHelpers.Extensions;

public static class Base64Extensions
{
    /// <summary>Write/Overwrite a file with the binary contents of the base64 string</summary>
    public static void DecodeBase64ToFile(this string base64string, string filename, bool overwrite = true)
    {
        if (overwrite && File.Exists(filename))
            File.Delete(filename);
        File.WriteAllBytes(filename, Convert.FromBase64String(base64string));
    }
    /// <summary>Return a base64 string from a stream.</summary>
    public static string EncodeAsBase64(this Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Convert.ToBase64String(ms.GetBuffer(), Base64FormattingOptions.InsertLineBreaks);
    }
}