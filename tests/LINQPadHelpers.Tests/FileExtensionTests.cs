using FluentAssertions;
using LINQPadHelpers.Extensions;

namespace LINQPadHelpers.Tests;

public class FileExtensionTests
{
    [Fact]
    public void AsFileSystemInfo_OnDirectoryPath_ReturnsDirectoryInfo()
    {
        var p = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                           .AsFileSystemInfo();
        p.Should().BeOfType<DirectoryInfo>();
    }

    [Fact]
    public void AsFileSystemInfo_OnFilePath_ReturnsFileInfo()
    {
        var path = Path.GetTempFileName();
        var fileInfo = path.AsFileSystemInfo();
        fileInfo.Should().NotBeNull();
        fileInfo.Should().BeOfType<FileInfo>();
        fileInfo!.Exists.Should().BeTrue();
        fileInfo.Delete();
    }
    
    [Fact]
    public void AsFileSystemInfo_OnNonexistentPath_ReturnsNull()
    {
        var path = "nonexistent/path";
        var fileInfo = path.AsFileSystemInfo();
        fileInfo.Should().BeNull();
    }
    
    [Fact]
    public void EnumerateFiles_OnDirectory_ReturnsAllFiles()
    {
        var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var directoryInfo = directoryPath.AsFileSystemInfo();
        var files = directoryInfo!.EnumerateFiles();
        files.Should().NotBeEmpty();
    }
    
    [Fact]
    public void ReadAllBytes_OnExistingFile_ReturnsByteArray()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "Hello World");
        var fileInfo = new FileInfo(path);
        var bytes = fileInfo.ReadAllBytes();
        bytes.Should().NotBeEmpty();
    }

    [Fact]
    public void ReadToEnd_OnExistingFile_ReturnsString()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "Hello World");
        path.ReadToEnd().Should().Be("Hello World");
        File.Delete(path);
    }

    [Fact]
    public void ReadToEnd_OnBadPath_ReturnsEmptyString()
    {
        var badPath = "bad/path";
        badPath.ReadToEnd().Should().BeEmpty();
    }
}
