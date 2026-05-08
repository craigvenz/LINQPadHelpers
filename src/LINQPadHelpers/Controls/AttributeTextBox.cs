using LINQPad.Controls;
using LINQPad.Controls.Core;
using System.Reactive.Linq;

namespace LINQPadHelpers.Controls;

public class AttributeTextBox : TextBox
{
    public AttributeTextBox(string initialText = "", 
                            string width = "30em", 
                            Action<TextBox>? onTextInput = null, 
                            Dictionary<string, string>? attributes = null,
                            IHtmlElementWrapper? suppliedWrapper = null)
        : this("text", initialText, width, onTextInput)
    {
        var htmlElementWrapper = suppliedWrapper ?? new DefaultHtmlElementWrapper(this, c => c.HtmlElement);
        if (attributes == null)
            return;
        foreach (var attr in attributes)
            htmlElementWrapper[attr.Key] = attr.Value;
    }
    protected AttributeTextBox(string type, string initialText, string width, Action<TextBox>? onTextInput) 
        : base(type, initialText, width, onTextInput)
    {
    }
    public IObservable<System.Reactive.EventPattern<object>> GetObservable() 
        => Observable.FromEventPattern(o => TextInput += o, o => TextInput -= o);
}
public interface IHtmlElementWrapper
{
    string this[string attributeName] { get; set; }    
}
internal class DefaultHtmlElementWrapper(Control control, Func<Control, HtmlElement> getControlHtmlElement)
    : IHtmlElementWrapper
{
    public string this[string attributeName]
    {
        get => getControlHtmlElement(control)[attributeName];
        set => getControlHtmlElement(control)[attributeName] = value;
    }
}

