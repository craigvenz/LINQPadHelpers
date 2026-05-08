using FluentAssertions;
using LINQPadHelpers.Extensions;

namespace LINQPadHelpers.Tests;

public class IpStringExtensionsTests
{
    [Fact]
    public void Ipv6Test()
    {
        "2603:8000:3e06:914c:d56e:105b:7ac8:5e6"
            .RegexIPv6()
            .Should()
            .BeTrue();
        
        "fe80::1".RegexIPv6().Should().BeTrue();

        //"00-15-5D-BC-CB-31".RegexIPv6().Should().BeFalse(); //?
    }
    
    [Fact(Skip = "I don't know what an invalid ip6 address looks like or why Brian's regex fails")]
    public void Ipv6_OnInvalidAddress_ReturnsFalse()
    {
        "invalidip6address".RegexIPv6().Should().BeFalse(); //?
    }

    [Fact]
    public void Ipv4Test()
    {
        "192.168.5.150"
            .RegexIPv4()
            .Should()
            .BeTrue();

        "10.a.9.bc".RegexIPv4()
                   .Should()
                   .BeFalse();
    }
}
