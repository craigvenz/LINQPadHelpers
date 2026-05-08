using System.Reactive.Linq;
using System.Reflection;

namespace LINQPadHelpers.Extensions;

public static class LinqpadControlExtensions
{
    /// <summary>Debounce a Linqpad html control event. Using the delay specified, all events specified by the string eventName are</summary>
    public static T Debounce<T>(this T control, string eventName, TimeSpan delay, Action<T> action, out IDisposable unsubHandle) where T : global::LINQPad.Controls.Control
    {
        var eventInfo = control.GetType()
                               .GetEvent(eventName, BindingFlags.Public 
                                                    | BindingFlags.NonPublic 
                                                    | BindingFlags.Instance 
                                                    | BindingFlags.IgnoreCase)
                        ?? throw new ArgumentException($"Event not found: {eventName}", nameof(eventName));
        unsubHandle = Observable.FromEventPattern(o => eventInfo.AddEventHandler(control, o), 
                                                  o => eventInfo.RemoveEventHandler(control, o))
                                .Throttle(delay)
                                .Subscribe(_ => action(control));
        return control;
    }
}
