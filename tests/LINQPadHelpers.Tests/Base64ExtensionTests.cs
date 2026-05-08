using FluentAssertions;
using LINQPadHelpers.Extensions;

namespace LINQPadHelpers.Tests
{
    public class Base64ExtensionTests
    {
        [Fact]
        public void DecodeBase64ToFile_Test()
        {
            var temp = new FileInfo(Path.GetTempFileName());
            "YWJjZGVmZ2g=".DecodeBase64ToFile(temp.FullName);
            temp.Exists.Should().BeTrue();
            temp.ReadToEnd().Should().Be("abcdefgh");
            temp.Delete();
        }

        [Fact]
        public void EncodeAsBase64()
        {
            new MemoryStream("abcdefgh"u8.ToArray()).EncodeAsBase64()
                                                    .Should()
                                                    .Be(
"""
YWJjZGVmZ2gAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==
""");
        }
    }
}
