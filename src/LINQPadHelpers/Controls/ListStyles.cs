using System.ComponentModel;

namespace LINQPadHelpers.Controls;

public enum ListStyles
{
    /// <summary>
    /// No item marker is shown.
    /// </summary>
    None,
    /// <summary>
    /// A filled circle (default value)
    /// </summary>
    Disc,
    /// <summary>
    /// A hollow circle.
    /// </summary>
    Circle, 
    /// <summary>
    /// A filled square.
    /// </summary>
    Square,  
    /// <summary>
    /// Decimal numbers, beginning with 1.
    /// </summary>
    Decimal, 
    /// <summary>
    /// Decimal numbers, padded by initial zeros.
    /// </summary>
    [Description("decimal-leading-zero")]
    DecimalLeadingZero,
    /// <summary>
    /// Lowercase roman numerals.
    /// </summary>
    [Description("lower-roman")]
    LowerRoman,
    /// <summary>
    /// Uppercase roman numerals.
    /// </summary>
    [Description("upper-roman")]
    UpperRoman,
    /// <summary>
    /// Lowercase ASCII letters.
    /// </summary>
    [Description("lower-alpha")] 
    LowerAlpha, 
    /// <summary>
    /// Lowercase ASCII letters.
    /// </summary>
    [Description("lower-latin")] 
    LowerLatin,
    /// <summary>
    /// Uppercase ASCII letters.
    /// </summary>
    [Description("upper-alpha")] 
    UpperAlpha, 
    /// <summary>
    /// Uppercase ASCII letters.
    /// </summary>
    [Description("upper-latin")] 
    UpperLatin,
    /// <summary>
    /// Symbol indicating that a disclosure widget such as &lt;details&gt; is opened.
    /// </summary>
    [Description("disclosure-open")]
    DisclosureOpen,
    /// <summary>
    /// Symbol indicating that a disclosure widget such as &lt;details&gt; is closed.
    /// </summary>
    [Description("disclosure-closed")]
    DisclosureClosed
}
