using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Humanizer;
using LINQPad;
using LINQPad.Controls;
using EnumsNET;

namespace LINQPadHelpers.Controls;

/// <summary>Various extensions for linqpad control display</summary>
public static class ObjectExtensions
{
    private static object? GetValue(MemberInfo memberInfo, object target)
        => memberInfo switch
        {
            FieldInfo f => f.GetValue(target),
            PropertyInfo p => p.GetValue(target),
            _ => throw new NotSupportedException()
        };

    private static IEnumerable<MemberInfo> GetPropertiesAndFields(Type t)
        => t.GetProperties().Concat(t.GetFields().Cast<MemberInfo>());

    private static bool IsSimpleType(MemberInfo mi) => mi switch
    {
        FieldInfo f => IsSimpleType(f.FieldType),
        PropertyInfo p => IsSimpleType(p.PropertyType),
        _ => throw new ArgumentOutOfRangeException(nameof(mi), mi, null)
    };
    private static bool IsSimpleType(Type t) => t.IsValueType || t == typeof(string);
    /// <summary>Override for OnDemand - if we were passed a linqpad control or linqpad dumpable object, just show it, don't make it on demand.</summary>
    public static DumpContainer OnDemandIfNotLinqpadControl(this object? obj, string? label = null)
    {
        var type = obj?.GetType();
        if (obj != null && (type!.IsAssignableTo(typeof(Control)) || type.Namespace == "LINQPad.ObjectGraph"))
            return new DumpContainer(obj);
        return obj.OnDemand(label);
    }

    /// <summary>Moved ToTableByProperty default row generation to its own function for reuse.</summary>
    public static TableRow DefaultToTableRow(this object obj)
        => new(from x in GetPropertiesAndFields(obj.GetType())
               let isSimple = IsSimpleType(x)
               let asObject = GetValue(x, obj)
               let asString = asObject?.ToString()
               orderby isSimple ? 0 : 1
               select new TableCell(isSimple ? asString.ToSpan()
                                             : OnDemandIfNotLinqpadControl(asObject, x.Name)));

    /// <summary>Moved ToTableByProperty header functionality to its own function for reuse.</summary>
    public static TableRow DefaultHeader(this object obj)
    {
        var properties = GetPropertiesAndFields(obj.GetType());
        return new TableRow(isHeader: true,
                            (from x in properties
                             orderby IsSimpleType(x) ? 0 : 1
                             select (Control)new TableCell(true, new Label(x.Name)))
                            .ToArray());
    }

    /// <summary>Creates a table with header from an object, showing properties in declared order, value types first, then complex types as Util.OnDemand links.</summary>
    public static Table ToTableByProperty(this object obj, bool withHeader = true)
    {
        var header = obj.DefaultHeader();
        var data = obj is ITableDisplay itd ? itd.AsTableRow() : obj.DefaultToTableRow();
        var rows = new List<TableRow> { data };
        if (withHeader)
            rows.Insert(0, header);
        return new Table(rows);
    }
    /// <summary>Returns a Span control for this object. Returns green-italics 'null' for null values.</summary>
    public static Control ToSpan(this object? o) => o == null
                                                        ? new Span(new DumpContainer(Util.Metatext("null")))
                                                        : new Span(new DumpContainer(o));
    /// <summary>Returns a Span control for this Func.</summary>
    public static Control ToSpan<T>(this Func<T> valueProducer) => valueProducer().ToSpan();
    /// <summary>Return a Table control for this enumerable, optionally specifying columns to include.</summary>
    public static Table ToTable<T>(this IEnumerable<T> data, IEnumerable<string>? columns = null)
    {
        var type = typeof(T);
        var list = data.ToList();
        Dictionary<Type, Dictionary<string, MemberInfo>> dict;
        if (type == typeof(object))
        {
            var groups = from item in list
                         group item by item.GetType() into allTypes
                         select new { Type = allTypes.Key, MemberInfo = MemberDictionary(allTypes.Key) };
            dict = groups.ToDictionary(k => k.Type, v => v.MemberInfo);
        }
        else
        {
            dict = new Dictionary<Type, Dictionary<string, MemberInfo>>
                   {
                       { type, MemberDictionary(type) }
                   };
        }

        var columnHeadings = (columns ??
                              (from typeAndMembers in dict
                               from members in typeAndMembers.Value
                               select members.Key)).ToList();
        var header = new TableRow(isHeader: true,
                                  children: from column in columnHeadings
                                            let name = !string.IsNullOrEmpty(column) ? column.ToSpan() : new Literal("&nbsp;")
                                            select new TableCell(isHeader: true, name));

        var rows = from item in list
                   let cells =
                       from columnHeading in columnHeadings
                       let typeDict = dict[columnHeading?.GetType() ?? typeof(object)]
                       let hasValue = !string.IsNullOrEmpty(columnHeading) && typeDict.ContainsKey(columnHeading)
                       let display = hasValue ? GetValue(typeDict[columnHeading], item).ToSpan() : new Literal("&nbsp;")
                       select new TableCell(display)
                   select new TableRow(cells);
        return new Table(rows: rows.Prepend(header));

        static Dictionary<string, MemberInfo> MemberDictionary(Type t)
            => GetPropertiesAndFields(t).ToDictionary(k => k.Name, v => v);
    }

    /// <summary>Quick hack - dump something as a table with a header description</summary>
    public static Table Describe<T>(this T item, string description) where T : Control =>
        new(rows: [new TableRow(isHeader: true, new Label(description)), new TableRow(item)]);

    /// <summary>Shortcut for Util.OnDemand</summary>
    public static DumpContainer OnDemand<T>(this Func<T> valueProducer, string? label = null) => Util.OnDemand(label ?? valueProducer?.ToString() ?? "null", valueProducer);
    /// <summary>Shortcut for Util.OnDemand</summary>
    public static DumpContainer OnDemand<T>(this T obj, string? label = null) => Util.OnDemand(label ?? obj?.ToString() ?? "null", () => obj);
    /// <summary>Debounce on a textbox control.</summary>
    public static IDisposable Debounce(TextBox control, Action<string> action, TimeSpan? delay = null)
        => Observable.FromEventPattern(o => control.TextInput += o, o => control.TextInput -= o)
                     .Throttle(delay ?? 1.5.Seconds())
                     .Select(_ => control.Text.Trim())
                     .Where(x => x.Length >= 3)
                     .Subscribe(action);

    private static TimeSpan DefaultTimer(TimeSpan? t) => t ?? 1.Seconds();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "I want it this way for a reason.")]
    private static DumpContainer CreatePreloadedObservable<T>(Func<T> value, IObservable<T> sequence) =>
        new LatestDumpContainer<T>(Observable.Empty<T>()
                                             .Prepend(value())
                                             .Merge(sequence));

    private static DumpContainer Timer(Func<string> produceValue, TimeSpan? refreshTime) =>
        CreatePreloadedObservable<string>(produceValue,
                                          from _ in Observable.Interval(DefaultTimer(refreshTime))
                                          select produceValue());
    /// <summary>Show a Func&lt;TimeSpan&gt; as an observable control that updates in place.</summary>
    public static DumpContainer UpdateInPlace(this Func<TimeSpan> t, TimeSpan? refreshTime = null, int precision = 1) =>
        Timer(() => t().Humanize(precision), refreshTime);
    /// <summary>Show a datetime as an observable control that updates in place.</summary>
    public static DumpContainer UpdateInPlace(this DateTime t, TimeSpan? refreshTime = null) =>
        Timer(() => $"{t} - {t.Humanize()}", refreshTime);
    /// <summary>Show an arbitrary Func&lt;long,string&gt; as an observable control that updates in place.</summary>
    public static DumpContainer UpdateInPlaceFromAction(this Func<long, string> select, TimeSpan? refreshTime = null) =>
        Timer(() => select(0), refreshTime);
    /// <summary>Highlight a specific string everywhere inside table formatted data.</summary>
    public static object Highlight(this object data, string text, bool caseInsensitive = false)
    {
        var html = Util.ToHtmlString(data);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var cells = from tableCell in doc.DocumentNode.SelectNodes("//td") ?? Enumerable.Empty<HtmlNode>()
                    where tableCell.InnerText.Contains(text, StringComparison.CurrentCultureIgnoreCase)
                    select tableCell;

        foreach (var cn in cells)
            cn.InnerHtml = Regex.Replace(cn.InnerHtml, $"({Regex.Escape(text)})", "<span class='highlight' style='background:yellow'>$1</span>", caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);

        return Util.RawHtml(doc.DocumentNode.OuterHtml);
    }

    private static string GetListStyleString(ListStyles listStyle) =>
        listStyle.GetAttributes()?
                 .OfType<DescriptionAttribute>()
                 .FirstOrDefault()?.Description
        ?? listStyle.ToString().ToLowerInvariant();
    /// <summary>
    /// Convert a list of items into an unordered list html control.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="listStyle"></param>
    /// <returns></returns>
    public static Literal ToHtmlList<T>(this IEnumerable<T> items, ListStyles listStyle = ListStyles.None)
        => new($"<ul style=\"list-style: {GetListStyleString(listStyle)}\">\n{(string.Join('\n', from x in items select $"<li>{x}</li>"))}</ul>");
    
    /// <summary>
    /// Applies an action to a control - meant to be used during construction of controls to do things in place fluently that can't be done using the current language features.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="control"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static T Customize<T>(this T control, Action<T> action) where T : Control
    {
        action(control);
        return control;
    }
}