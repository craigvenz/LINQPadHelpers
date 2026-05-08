using LINQPad.Controls;

namespace LINQPadHelpers.Controls;

public class CheckboxList<T> where T : class
{
    public CheckboxList() => Items = [];
    public CheckboxList(IEnumerable<T> items)
        => Items = items.Select(x => new CheckboxSelector<T>(x)).ToList();
    public CheckboxList(IEnumerable<T> items, Action<T> onClick) : this(items)
    {
        foreach (var i in Items)
            i.Select.Click += (_, _) => onClick(i.Item);
    }
    public List<CheckboxSelector<T>> Items { get; }
    public object ToDump()
    {
        if (Items.Count == 0)
            return new Table();
        var header = Items.First().Item.DefaultHeader();
        header.Cells.Insert(0, new TableCell(true, new Label("Select")));
        return new Table(Items.Cast<ITableDisplay>()
                              .Select(x => x.AsTableRow())
                              .Prepend(header));
    }
}