using FluentAssertions;
using LINQPadHelpers.Extensions;
using Newtonsoft.Json;

namespace LINQPadHelpers.Tests;

public class ConvenienceMethodsTests
{
    [Fact]
    public void CachedSerializerTest()
    {
        var serializer = ConvenienceMethods.CreateJsonSerializer();
        
        var serializer2 = ConvenienceMethods.CreateJsonSerializer();

        serializer.Should().BeSameAs(serializer2);
    }

    [Fact]
    public void CustomDefaultSettingsTest()
    {
        var settings = ConvenienceMethods.CustomizeDefaultSettings(ss =>
                                                                   {
                                                                       ss.Formatting = Formatting.None;
                                                                       ss.NullValueHandling = NullValueHandling.Include;
                                                                   });
        settings.Formatting.Should().Be(Formatting.None);
        settings.NullValueHandling.Should().Be(NullValueHandling.Include);
        settings.StringEscapeHandling.Should().Be(StringEscapeHandling.EscapeNonAscii);
        settings.ContractResolver.Should().BeOfType<ConvenienceMethods.OrderedContractResolver>();
    }

    private class Test1
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
    }
    
    [Fact]
    public void OrderedContractResolverTest()
    {
        var obj = new Test1 { Name = "some name", Age = 10 };
        var json = JsonConvert.SerializeObject(obj, ConvenienceMethods.CustomDefaultSettings);
        json.Should()
            .Be(
                """
                {
                  "Age": 10,
                  "Name": "some name"
                }
                """
               );

        json =
            JsonConvert.SerializeObject(obj,
                                        ConvenienceMethods.CustomizeDefaultSettings(ss => ss.ContractResolver = null));
        json.Should()
            .Be(
                """
                {
                  "Name": "some name",
                  "Age": 10
                }
                """
               );
    }

    [Fact]
    public void ReadAndWriteJsonZipTest()
    {
        var obj = new Test1() { Name = "some name", Age = 10 };
        var tempFile = Path.GetTempFileName();
        ConvenienceMethods.WriteJsonZip(tempFile, obj);
        var deserializedObj = ConvenienceMethods.ReadJsonZip<Test1>(tempFile);
        obj.Should().BeEquivalentTo(deserializedObj);
        File.Delete(tempFile);
    }

    [Fact]
    public void DesktopFolderPathTest()
    {
        var desktopFolderPath = ConvenienceMethods.DesktopFolderPath("filename.txt");
        desktopFolderPath.Should().EndWith("Desktop\\filename.txt");
    }
}
