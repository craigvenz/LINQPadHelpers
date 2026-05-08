using LINQPadHelpers.Controls;
using Moq;

namespace LINQPadHelpers.Tests;

public class AttributeTextBoxTests
{
    [Fact]
    public void AttributesAreSet_WhenPassedToConstructor()
    {
        var mockHtmlElementWrapper = new Mock<IHtmlElementWrapper>();
        var attributes = new Dictionary<string, string>()
                                                {
                                                    { "attribute1", "value1" },
                                                    { "attribute2", "value2" }
                                                };
        _ = new AttributeTextBox(attributes:attributes, suppliedWrapper:mockHtmlElementWrapper.Object);
        mockHtmlElementWrapper.VerifySet(x => x["attribute1"] = "value1", Times.Once());
        mockHtmlElementWrapper.VerifySet(x => x["attribute2"] = "value2", Times.Once());
    }
    [Fact]
    public void TestAttributeTextBox_NoAttributes()
    {
        var mockHtmlElementWrapper = new Mock<IHtmlElementWrapper>();
        _ = new AttributeTextBox(suppliedWrapper:mockHtmlElementWrapper.Object);
        mockHtmlElementWrapper.VerifySet(x => x[It.IsAny<string>()] = It.IsAny<string>(), Times.Never());
    }
}
