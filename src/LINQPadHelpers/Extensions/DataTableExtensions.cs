using System.Data;
using System.Data.SqlClient;

namespace LINQPadHelpers.Extensions;

public static class DataTableExtensions
{
    /// <summary>Run this SQL against the supplied connection string and return a DataTable.</summary>
    public static DataTable GetDataTable(this string sql, string connectionString)
        => RunQuery(sql, connectionString, da =>
                                           {
                                               var dt = new DataTable();
                                               da.Fill(dt);
                                               return dt;
                                           });

    /// <summary>Run this SQL statement(s) against the supplied connection string and return a DataSet.</summary>
    public static DataSet GetDataSet(this string sql, string connectionString) 
        => RunQuery(sql, connectionString, da =>
             {
                 var dt = new DataSet();
                 da.Fill(dt);
                 return dt;
             });

    private static T RunQuery<T>(string sql, string connectionString, Func<SqlDataAdapter, T> fillFunc)
    {
        using var conn = new SqlConnection(connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        var da = new SqlDataAdapter(cmd);
        return fillFunc(da);
    }
    
    /// <summary>Copy the sequence into a new DataTable.</summary>
    public static DataTable CopyToDataTable<T>(this IEnumerable<T> source) where T : notnull
        => new ObjectShredder<T>().Shred(source, new DataTable(typeof(T).Name), null);

    /// <summary>Copy the sequence into the supplied DataTable instance.</summary>
    public static DataTable CopyToDataTable<T>(this IEnumerable<T> source, DataTable table, LoadOption options = LoadOption.OverwriteChanges) where T : notnull
        => new ObjectShredder<T>().Shred(source, table, options);
}