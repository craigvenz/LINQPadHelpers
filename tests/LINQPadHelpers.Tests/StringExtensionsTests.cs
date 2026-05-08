using System.Text;
using FluentAssertions;
using LINQPadHelpers.Extensions;

namespace LINQPadHelpers.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void ToGuidList()
    {
        var values = """

            2DC0ED33-543F-4407-85A2-1BCF6EC957E5
            2DC0ED33-543F-4407-85A2-1BCF6EC957E5,
            9EB60D13-BF03-4ADD-AC77-4A4EA729D58E
            9EB60D13-BF03-4ADD-AC77-4A4EA729D58E|
            3FEF464E-01DB-45F1-B227-862B5CD516D5
            3FEF464E-01DB-45F1-B227-862B5CD516D5
            3FEF464E-01DB-45F1-B227-862B5CD516D5
            3FEF464E-01DB-45F1-B227-862B5CD516D5
            3A490F03-A599-4531-83D2-9D6C5DA540BB
            3A490F03-A599-4531-83D2-9D6C5DA540BB
            3A490F03-A599-4531-83D2-9D6C5DA540BB
            3A490F03-A599-4531-83D2-9D6C5DA540BB

            """.ToGuidList()
               .Distinct();
        values.Should()
              .BeEquivalentTo([
                                  new Guid("2DC0ED33-543F-4407-85A2-1BCF6EC957E5"),
                                  new Guid("9EB60D13-BF03-4ADD-AC77-4A4EA729D58E"),
                                  new Guid("3FEF464E-01DB-45F1-B227-862B5CD516D5"),
                                  new Guid("3A490F03-A599-4531-83D2-9D6C5DA540BB")
                              ]);
    }

    [Fact]
    public void String_GetValueOrDefault()
    {
        "".GetValueOrDefault("test").Should().Be("test");
        "test2".GetValueOrDefault("abcd").Should().Be("test2");
        ((string)null!).GetValueOrDefault("test").Should().Be("test");
    }

    [Fact]
    public void String_OnNotEmptyString()
    {
        "".OnNotEmptyString(Transform).Should().BeEmpty();
        "bcd".OnNotEmptyString(Transform).Should().Be("bcd");
        "ABCD".OnNotEmptyString(Transform).Should().Be("ABCD");
        return;

        static string Transform(string value) => value.Replace("A", "a");
    }

    [Fact]
    public void String_Coalesce()
    {
        string[] v = ["","a","b"];
        v.Coalesce().Should().Be("a");
        "".Coalesce("b").Should().Be("b");
        "b".Coalesce("c","d").Should().Be("b");
    }

    [Fact]
    public void String_Truncate()
    {
        "abcdefg".Truncate(2).Should().Be("ab...");
        "abcdefgh".Truncate(5).Should().Be("abcde...");
        ((string)null!).Truncate(2).Should().Be("");
        //"".Truncate(10).Should().Be("");
    }

    [Fact]
    public void String_JoinWhere()
    {
        var names = new List<string> { "John", "Jane", "Alice", "Bob" };
        var result = names.JoinWhere(", ", name => name.StartsWith('J'));
        result.Should().Be("John, Jane");
    }

    [Fact]
    public void String_JoinWhereNotEmpty()
    {
        var names = new List<string> { "John", "Jane", "", "Alice", "Bob" };
        var result = names.JoinWhereNotEmpty();
        result.Should().Be("John, Jane, Alice, Bob");
    }
    
    [Fact]
    public void String_ToIsoString()
    {
        var date = new DateTime(2022, 1, 1, 12, 0, 0);
        var isoString = date.ToIsoString();
        isoString.Should().Be("20220101T120000");
    }
    
    [Fact]
    public void String_ToPrintableList()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };
        var printableList = list.ToPrintableList();
        printableList.Should().Be("IEnumerable<Int32> [1, 2, 3, 4, 5]");
    }

    [Fact]
    public void String_ToHexString()
    {
        new byte[] { 1, 3, 5, 7, 9, 11 }
            .ToHexString()
            .Should()
            .Be("01030507090B");
    }

    [Fact]
    public void String_ComputeHash_WithDefaults()
    {
        "input".ComputeHash()
               .Should()
               .Be("C96C6D5BE8D08A12E7B5CDC1B207FA6B2430974C86803D8891675E76FD992C20");
    }

    [Fact]
    public void String_ComputeHash_WithDifferentMethods()
    {
        "input".ComputeHash(Encoding.UTF32.GetBytes)
               .Should()
               .Be("916B8D722CAF6508233CE7CD4E236171A637B11D51D9DF5AFDED253DEA17FF65");
    }
    
    [Fact]
    public void String_ComputeHash_WithCustomHashFunction()
    {
        "input".ComputeHash(hashFunc: System.Security.Cryptography.MD5.HashData)
               .Should()
               .Be("A43C1B0AA53A0C908810C06AB1FF3967");
    }

    [Fact]
    public void Stream_ComputeHash()
    {
        var stream = new MemoryStream("input"u8.ToArray());
        var hash = stream.ComputeHash();
        hash.Should().Be("C96C6D5BE8D08A12E7B5CDC1B207FA6B2430974C86803D8891675E76FD992C20");
    }

    [Fact]
    public void String_CleanFileName()
    {
        "file&1-'?.txt".CleanFileName().Should().Be("file-1---.txt");
    }
}
