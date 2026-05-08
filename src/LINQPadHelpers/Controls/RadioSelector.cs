using LINQPad.Controls;

namespace LINQPadHelpers.Controls;

public class RadioSelector<T>(string group, string label, T item) where T : class
{
    public T Item => item;
    public RadioButton Select { get; } = new(group, label);
    public object ToDump() => new { Select, Item };
}
