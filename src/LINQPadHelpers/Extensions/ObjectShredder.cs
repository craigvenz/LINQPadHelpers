using System.Data;
using System.Reflection;

namespace LINQPadHelpers.Extensions;

/// <summary>https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/implement-copytodatatable-where-type-not-a-datarow</summary>
internal class ObjectShredder<T> where T : notnull
{
    private readonly FieldInfo[] _fi;
    private readonly PropertyInfo[] _pi;
    private readonly Dictionary<string, int> _ordinalMap = [];

    // ObjectShredder constructor.
    public ObjectShredder()
    {
        var type = typeof(T);
        _fi = type.GetFields();
        _pi = type.GetProperties();
    }

    /// <summary>
    /// Loads a DataTable from a sequence of objects.
    /// </summary>
    /// <param name="source">The sequence of objects to load into the DataTable.</param>
    /// <param name="table">The input table. The schema of the table must match that 
    /// the type T.  If the table is null, a new table is created with a schema 
    /// created from the public properties and fields of the type T.</param>
    /// <param name="options">Specifies how values from the source sequence will be applied to 
    /// existing rows in the table.</param>
    /// <returns>A DataTable created from the source sequence.</returns>
    public DataTable Shred(IEnumerable<T> source, DataTable table, LoadOption? options)
    {
        // Load the table from the scalar sequence if T is a primitive type.
        if (typeof(T).IsPrimitive)
        {
            return ShredPrimitive(source, table, options);
        }

        // Initialize the ordinal map and extend the table schema based on type T.
        table = ExtendTable(table, typeof(T));

        // Enumerate the source sequence and load the object values into rows.
        table.BeginLoadData();
        using (var e = source.GetEnumerator())
        {
            while (e.MoveNext())
            {
                if (options != null)
                {
                    table.LoadDataRow(ShredObject(table, e.Current), (LoadOption)options);
                }
                else
                {
                    table.LoadDataRow(ShredObject(table, e.Current), true);
                }
            }
        }
        table.EndLoadData();

        // Return the table.
        return table;
    }

    public static DataTable ShredPrimitive(IEnumerable<T> source, DataTable? table, LoadOption? options)
    {
        // Create a new table if the input table is null.
        table ??= new DataTable(typeof(T).Name);

        if (!table.Columns.Contains("Value"))
        {
            table.Columns.Add("Value", typeof(T));
        }

        // Enumerate the source sequence and load the scalar values into rows.
        table.BeginLoadData();
        using (var e = source.GetEnumerator())
        {
            var values = new object[table.Columns.Count];
            while (e.MoveNext())
            {
                values[table.Columns["Value"]?.Ordinal ?? 0] = e.Current;

                if (options != null)
                {
                    table.LoadDataRow(values, (LoadOption)options);
                }
                else
                {
                    table.LoadDataRow(values, true);
                }
            }
        }
        table.EndLoadData();

        // Return the table.
        return table;
    }

    public object[] ShredObject(DataTable table, T instance)
    {
        var fi = _fi;
        var pi = _pi;

        if (instance.GetType() != typeof(T))
        {
            // If the instance is derived from T, extend the table schema
            // and get the properties and fields.
            ExtendTable(table, instance.GetType());
            fi = instance.GetType().GetFields();
            pi = instance.GetType().GetProperties();
        }

        // Add the property and field values of the instance to an array.
        var values = new object[table.Columns.Count];
        foreach (var f in fi)
        {
            values[_ordinalMap[f.Name]] = f.GetValue(instance)!;
        }

        foreach (var p in pi)
        {
            values[_ordinalMap[p.Name]] = p.GetValue(instance, null)!;
        }

        // Return the property and field values of the instance.
        return values;
    }

    public DataTable ExtendTable(DataTable table, Type type)
    {
        // Extend the table schema if the input table was null or if the value 
        // in the sequence is derived from type T.            
        foreach (var f in type.GetFields())
        {
            if (!_ordinalMap.ContainsKey(f.Name))
            {
                // Add the field as a column in the table if it doesn't exist
                // already.
                var dc = table.Columns.Contains(f.Name) ? table.Columns[f.Name]
                             : table.Columns.Add(f.Name, Nullable.GetUnderlyingType(f.FieldType) ?? f.FieldType);

                // Add the field to the ordinal map.
                _ordinalMap.Add(f.Name, dc!.Ordinal);
            }
        }
        foreach (var p in type.GetProperties())
        {
            if (_ordinalMap.ContainsKey(p.Name))
                continue;
            
            // Add the property as a column in the table if it doesn't exist already.
            var dc = table.Columns.Contains(p.Name) 
                         ? table.Columns[p.Name] 
                         : table.Columns
                                .Add(p.Name, 
                                     Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);

            // Add the property to the ordinal map.
            _ordinalMap.Add(p.Name, dc!.Ordinal);
        }

        // Return the table.
        return table;
    }
}
