using LINQPad;
using System.Reflection;

namespace LINQPadHelpers.Extensions;

public static class TypeNameExtensions
{
    private static string TypeNameImpl(Type? t)
    {
        if (t == null)
            return "(null)";
        var n = Nullable.GetUnderlyingType(t);
        return t.IsGenericType ? $"{t.Name.Split('`', StringSplitOptions.RemoveEmptyEntries).First()}<{string.Join(",", t.GenericTypeArguments.Select(tt => tt.Name))}>" : t.Name;
    }

    /// <summary>Get readable name of the type, generic or not</summary>
    public static string GetTypeName(this object? o) => o is Type type ? TypeNameImpl(type) : TypeNameImpl(o?.GetType());
    /// <summary>From Aguacongas - returns true if the type implements the supplied generic interface.</summary>
    public static bool ImplementsGenericInterface(this Type type, Type interfaceType) => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(info => info.IsGenericType(interfaceType))
                                                                                                                           || type.GetTypeInfo().IsAssignableTo(interfaceType);
    /// <summary>From Aguacongas - returns true if the type matches the supplied generic type.</summary>
    public static bool IsGenericType(this Type type, Type genericType) => type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
    /// <summary>Print the contents of static fields of a type.</summary>
    /// <remarks>https://stackoverflow.com/a/54191033/223942</remarks>
	public static void DumpStatic(this Type type, string? description = null)
    {
        const BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        var fields = type.GetFields(bf)
                         .Select(x => new
                                      {
                                          type = "Field",
                                          x.Name,
                                          x.IsPublic,
                                          Value = DumpValue(x.GetValue(null))
                                      });
        var props = type.GetProperties(bf)
                        .Select(x => new
                        {
                            type = "Property",
                            x.Name,
                            IsPublic = x.GetAccessors().Any(a => a.IsPublic),
                            Value = DumpValue(x.GetValue(null))
                        });
        fields.Concat(props)
              .OrderBy(x => x.Name)
              .Select(x => x.IsPublic ? x : Util.WithStyle(x, "background-color: #333333"))
              .Dump(description: description ?? $"{type.FullName} static fields and properties", exclude: "IsPublic");
        return;

        static object DumpValue(object? o)
        {
            if (o?.GetType()?.IsValueType == true || o is string)
                return o;
            return Util.RawHtml(Util.ToHtmlString(true, o));
        }
    }

    public static bool IsNullable(this Type t) 
        => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>Return a tuple (string name, object value, Type t) from all the properties on an object. Optionally perform an action with each. Reminder that it's returning an enumerable which is lazy evaluated.</summary>
    public static IEnumerable<(string name, object? value, Type t)> GetPropertyValues(this object obj, Action<string, object?, Type>? actions = null)
    {
        foreach (var p in obj.GetType().GetProperties())
        {
            var t = p.PropertyType;
            var n = p.Name;
            var v = p.GetValue(obj, null) ?? (t.IsValueType ? Activator.CreateInstance(t) : null);
            actions?.Invoke(n, v, t);
            yield return (n, v, t);
        }
    }
    /// <summary>Return a tuple (string name, object value, Type t) from all the fields on an object. Optionally perform an action with each. Reminder that it's returning an enumerable which is lazy evaluated.</summary>
    public static IEnumerable<(string name, object? value, Type t)> GetFieldValues(this object obj, Action<string, object?, Type>? actions = null)
    {
        foreach (var p in obj.GetType().GetFields())
        {
            var t = p.FieldType;
            var n = p.Name;
            var v = p.GetValue(obj) ?? (t.IsValueType ? Activator.CreateInstance(t) : null);
            actions?.Invoke(n, v, t);
            yield return (n, v, t);
        }
    }
    public static PropertyInfo? GetStaticProperty(this Type type, string name) => type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    public static FieldInfo? GetStaticField(this Type type, string name) => type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    /// <summary>Call GetValue on either a PropertyInfo or FieldInfo. Throws on any other type of MemberInfo.</summary>
    public static object? GetValue(this MemberInfo memberInfo, object? x) 
    => memberInfo switch
       {
              FieldInfo f => f.GetValue(x),
           PropertyInfo p => p.GetValue(x),
                        _ => throw new NotSupportedException()
       };
}