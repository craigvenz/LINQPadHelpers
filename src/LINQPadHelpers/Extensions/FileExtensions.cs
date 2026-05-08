using System.IO.Enumeration;

namespace LINQPadHelpers.Extensions;

public static class FileExtensions
{
    /// <summary>Returns a new FileSystemInfo for the path if it exists or else null.</summary>
    public static FileSystemInfo? AsFileSystemInfo(this string path) =>
        File.Exists(path) ? new FileInfo(path) :
        Directory.Exists(path) ? new DirectoryInfo(path) :
        null;
    /// <summary>Returns a new FileSystemInfo for the root and sub-path if it exists or else null.</summary>
    public static FileSystemInfo? AsFileSystemInfo(this string path, string root) =>
        Path.Combine(root, path).AsFileSystemInfo();

    /// <summary>Recurses the presented directory and returns all its files in all sub directories.</summary>
    public static IEnumerable<FileInfo> EnumerateFiles(this FileSystemInfo f, string searchPath = "*")
    {
        switch (f)
        {
            case FileInfo fileInfo when FileSystemName.MatchesSimpleExpression(searchPath, f.FullName):
                yield return fileInfo;
                break;
            case DirectoryInfo directoryInfo:
            {
                foreach (var fi in directoryInfo.EnumerateFiles(searchPath, SearchOption.AllDirectories))
                    yield return fi;
                break;
            }
        }
    }
    public static byte[] ReadAllBytes(this FileInfo file)
        => file.Exists ? File.ReadAllBytes(file.FullName) : [];
    /// <summary>Shortcut for checking for existence, creating a StreamReader and calling ReadToEnd.</summary>
    public static string ReadToEnd(this FileInfo file)
        => file.Exists ? new StreamReader(file.OpenRead())
                             .Using(f => f.ReadToEnd()) 
                         ?? string.Empty
               : string.Empty;
    /// <summary>Shortcut for creating a new FileInfo, checking for existence, creating a StreamReader and calling ReadToEnd.</summary>
    public static string ReadToEnd(this string file) => new FileInfo(file).ReadToEnd();
}
