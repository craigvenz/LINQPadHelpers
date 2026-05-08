using LINQPad;

namespace LINQPadHelpers.Controls;

/// <summary>DumpContainer override that takes an observable and updates its content when the observable updates.</summary>
public sealed class LatestDumpContainer<T> : DumpContainer, IDisposable
{
    private readonly IDisposable _subscription;
    public LatestDumpContainer(IObservable<T> content)
    {
        _subscription = content.Subscribe(OnNextAction);
        Style = "color:green";
    }
    private void OnNextAction(T item) => Content = item;
    public void Unsubscribe() => _subscription.Dispose();
    public void Dispose() => Unsubscribe();
}
