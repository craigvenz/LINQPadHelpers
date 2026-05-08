using FluentAssertions;
using LINQPadHelpers.Extensions;

namespace LINQPadHelpers.Tests;

public class DeconstructExtensionsTests
{
    [Fact]
    public void DeconstructWorksOnStringWithTwoElements()
    {
        const string input = "Hello World";
        var (first, second) = input.Split(' ');
        (first, second).Should().Be(("Hello","World"));
    }

    private class TestClass
    {
        public int SomeValue { get; set; }
        public string SomeString { get; set; } = null!;
    }
        
    [Fact]
    public void DeconstructOnListOfComplexTypesWorksAsExpected()
    {
        var data = new List<TestClass>
                   {
                       new() { SomeValue = 1, SomeString = "Test" },
                       new() { SomeValue = 2, SomeString = "Test2" }
                   };
        var (first, second) = data;
        first.Should().BeEquivalentTo(data[0]);
        second.Should().BeEquivalentTo(data[1]);
    }
        
    [Fact]
    public void DeconstructWorksOnStringWithThreeElements()
    {
        const string input = "Hello World C#";
        var (first, second, third) = input.Split(' ');
        first.Should().Be("Hello");
        second.Should().Be("World");
        third.Should().Be("C#");
    }
        
    [Fact]
    public void DeconstructWorksOnStringWithFourElements()
    {
        const string input = "Hello World C# Code";
        var (first, second, third, fourth) = input.Split(' ');
        first.Should().Be("Hello");
        second.Should().Be("World");
        third.Should().Be("C#");
        fourth.Should().Be("Code");
    }
        
    [Fact]
    public void DeconstructWorksOnStringWithFiveElements()
    {
        const string input = "Hello World C# Code Generator";
        var (first, second, third, fourth, fifth) = input.Split(' ');
        first.Should().Be("Hello");
        second.Should().Be("World");
        third.Should().Be("C#");
        fourth.Should().Be("Code");
        fifth.Should().Be("Generator");
    }
        
    [Fact]
    public void CallingDeconstructOnStringSplitWithOneElementReturnedDoesNotThrowException()
    {
        const string input = "Hello";
        var (first,second) = input.Split(' ');
        first.Should().Be("Hello");
        second.Should().BeNull();
    }

    [Fact]
    public void DeconstructThrowsArgumentNull()
    {
        Action act = () =>
                     {
                         List<string> nullList = null!;
                         var (first, second) = nullList;
                     };
        act.Should().Throw<ArgumentNullException>();
    }
}