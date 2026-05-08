using LINQPad;
using Microsoft.Extensions.Logging;
using LPControls = LINQPad.Controls;

namespace LINQPadHelpers.Logging;

internal struct MyLogEntry<TState>(LogLevel logLevel, 
                                   string category, 
                                   EventId eventId, 
                                   TState state, 
                                   Exception? exception, 
                                   Func<TState, Exception?, string> formatter, 
                                   bool utc)
{
    internal string Category = category;

    private readonly DateTimeOffset GetCurrentDateTime() => utc ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    private static string GetLogLevelString(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
        };
    public object ToDump()
    {
        if (exception == null && state == null)
            return Util.Metatext("null");
        var rows = new[]
                   {
                       new LPControls.TableRow(isHeader:true,
                                               new LPControls.Span(GetLogLevelString(logLevel)),
                                               new LPControls.Span(GetCurrentDateTime().ToString())),
                       new LPControls.TableRow(new DumpContainer(formatter(state,exception)))
                   };
        rows[1].Cells[0].HtmlElement.SetAttribute("colspan", "2");
        rows[1].Cells[0].Styles["word-wrap"] = "break-word";
        rows[1].Cells[0].Styles["max-width"] = "1000px";
        return new LPControls.Table(rows, true);
    }
}
