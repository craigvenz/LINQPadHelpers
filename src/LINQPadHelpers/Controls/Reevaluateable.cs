using LINQPad;
using LINQPad.Controls;

namespace LINQPadHelpers.Controls;

/// <summary>A dumpable control that takes a Func argument, and contains a pressable button that reevalues the function when pressed.</summary>
public class Reevaluateable
{
    public DumpContainer Display { get; } = new();
    public Button Trigger { get; }
    public required Func<object> Expression { get; set; } = () => "No expression defined";
    
    private readonly string? Label;
    
    public Reevaluateable(string? label = null)
    {
        Label = label;
        Trigger = new("↺", Run);
    }
    
    private void Run(Button btn) => Display.Content = Expression();
    
    public object ToDump()
    {
        Run(Trigger);
        var rows = new List<TableRow>()
                   {
                       new(Display, Trigger)
                   };
        if (Label != null)
        {
            var header = new TableRow(true, new Label(Label));
            header.Cells.First().HtmlElement.SetAttribute("colspan", "2");
            rows.Insert(0, header);
        }
        return new Table(rows);
    }
}
