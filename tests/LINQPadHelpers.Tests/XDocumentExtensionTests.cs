using System.Xml.Linq;
using FluentAssertions;
using LINQPadHelpers.Extensions;

namespace LINQPadHelpers.Tests;

public class XDocumentExtensionTests
{
    private readonly XDocument _d = new(new XElement("test",
                                                     new XElement("item",
                                                                  new XAttribute("id", 1)),
                                                     new XElement("item",
                                                                  new XAttribute("id", 2),
                                                                  new XElement("value", "test")),
                                                     new XElement("item", new XAttribute("id", 3),
                                                                  new XElement("intValue", 1)),
                                                     new XElement("item", new XAttribute("id", true),
                                                                  new XElement("intValue", "true")),
                                                     new XElement("item", new XAttribute("id", 5),
                                                                  new XElement("intValue", "1234.56")),
                                                     new XElement("item", new XAttribute("id", 6),
                                                                  new XElement("intValue", "1234.5678"))
                                                    )
                                       );

    [Fact]
    public void GetValueOrDefaultInt_ValueExists()
    {
        _d.Element("test")!.Elements("item").First().Attribute("id")!.GetValueOrDefault(0)
          .Should()
          .Be(1);
    }

    [Fact]
    public void GetValueOrDefaultInt_ValueNotExists()
    {
        _d.Element("test")!.Elements("item")
          .First()
          .GetValueOrDefault(0)
          .Should()
          .Be(0);
    }

    [Fact]
    public void GetValueOrDefaultString_ValueExists()
    {
        _d.Element("test")!.Elements("item")
          .FirstOrDefault(x => x.Attribute("id").GetValueOrDefault(0) == 2)
          .GetValueOrDefault("")
          .Should()
          .Be("test");
    }

    [Fact]
    public void GetValueOrDefaultBool_ValueExists()
    {
        _d.Element("test")!.Elements("item")
          .FirstOrDefault(x => x.Attribute("id").GetValueOrDefault<bool>())
          .GetValueOrDefault<bool>()
          .Should()
          .BeTrue();
    }

    [Fact]
    public void GetValueOrDefaultDecimal_ValueExists()
    {
        _d.Element("test")!
          .Elements("item")
          .FirstOrDefault(x => x.Attribute("id")
                                .GetValueOrDefault<int>() ==
                               5)
          .GetValueOrDefault<decimal>()
          .Should()
          .Be(1234.56m);
    }

    [Fact(Skip="confused")]
    public void GetValueOrDefaultDecimal_NotSureWhatIExpectedHere()
    {
        _d.Element("test")
          .Elements("item")
          .FirstOrDefault(x => x.Attribute("id")
                                .GetValueOrDefault<int>() == 5)
          .GetValueOrDefault<int>()
          .Should()
          .Be(1234);
    }
}
