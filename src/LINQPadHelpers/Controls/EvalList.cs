using System.Collections;
using LINQPad;
using LINQPad.Controls;

namespace LINQPadHelpers.Controls;

public class EvalList<T>(IEnumerable<Func<T>> data) : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator() => _list.Select(x => (T?)Invoke(x) ).GetEnumerator()!;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly string _label = string.Empty;
    private readonly List<Func<T>> _list = data.ToList();
    
    public EvalList(string label, IEnumerable<Func<T>> data) : this(data) => _label = label;

    private static object? Invoke(Func<T?> dataProducer) => Util.Try(dataProducer, _ => default);
    public object ToDump()
    {
        var tuples = _list.Select(x =>
                                  {
                                      var dumpContainer = new DumpContainer(Invoke(x));
                                      return new { dumpContainer, hyperLink = new Hyperlink("↺", _ => dumpContainer.Content = Invoke(x))};
                                  }).ToArray();

        var returnValue = tuples.Select(x => Util.HorizontalRun(true, x.dumpContainer, x.hyperLink));

        var verticalRun = new StackPanel(false,
                                         new Button("Rerun all", OnClick),
                                         new DumpContainer(returnValue));
        return !string.IsNullOrEmpty(_label) 
                   ? new FieldSet(_label, verticalRun) 
                   : verticalRun;

        void OnClick(Button _)
        {
            foreach (var (controls, valueProducer) in tuples.Zip(_list))
            {
                controls.dumpContainer.Content = Invoke(valueProducer);
            }
        }
    }
}
