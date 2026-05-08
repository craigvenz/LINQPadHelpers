using LINQPad.Controls;

namespace LINQPadHelpers.Controls;

/// <summary>Interface for defining a method to return a TableRow for dumping as a table</summary>
public interface ITableDisplay
{
    /// <summary></summary>
    TableRow AsTableRow();
}
