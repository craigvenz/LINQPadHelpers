using LINQPad;
using LINQPad.Controls;

namespace LINQPadHelpers.Controls;

public class CheckboxSelector<T>(T item) : ITableDisplay
    where T : class
{
    public CheckboxSelector(T item, Action<T> onClick) : this(item)
        => Select.Click += (_, _) => onClick(item);

    public CheckBox Select { get; init; } = new();

    public T Item => item;

    public TableRow AsTableRow()
    {
        if (item is not ITableDisplay td)
            return new TableRow(Select, new DumpContainer(item));
        var row = td.AsTableRow();
        row.Cells.Insert(0, new TableCell(Select));
        return row;
    }
    public object ToDump()
    {
        if (item is not ITableDisplay td) 
            return new { Select, Item = item };
        var row = td.AsTableRow();
        row.Cells.Insert(0, new TableCell(Select));
        return row;
    }
}
