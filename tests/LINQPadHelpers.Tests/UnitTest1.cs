namespace LINQPadHelpers.Tests;

public class UnitTest1
{
    [Fact]
    public async Task LINQPadTokenCredential_Test()
    {
        // I really dont know how to properly unit test this.
        //var credential = new LINQPadTokenCredential("", "");
    }
}



/*

#region tests
internal class testClass
{
    public int i { get; set; }
}
internal class testClassWithFields
{
    public int i;
    public int j;
}
#endregion

void Main()
{
    // Write code to test your extensions here. Press F5 to compile and run.
    "http://sdfgsdfsd.sdfsdf/sdfsdf.sdfs//".RegEx(@"/").Count().Dump();
    "http://sdfgsdfsd.sdfsdf/sdfsdf.sdfs//".RegExCount(@"/").Dump();
    "This is a test. Type a value?".GetUserInput("Test", "Canceled").Dump();
    "����������������������".Dump();

    typeof(XElement).GetTypeName().Dump("GetTypeName on typeof statement");
    new List<string>().GetTypeName().Dump("GetTypeName on an object");
    object? testObject = null;
    testObject.GetTypeName().Dump("GetTypeName on null");
    "����������������������".Dump();

    Task.FromResult(1).DumpAsync("test");

    "\n����������������������".Dump();
    "IPV4 Test".Dump();
    "����������������������".Dump();
    ("A.300.200.100".RegexIPv4().PassFalse()).Dump(); //Failing, A is not allowed
    ("0.0.0.0".RegexIPv4().PassTrue()).Dump();
    ("0.0.0.0,".RegexIPv4().PassFalse()).Dump(); //Failing, , is not allowed
    ("::1".RegexIPv6().PassTrue()).Dump();
    ("000:000:000:0.0.0.1".RegexIPv6().PassTrue()).Dump();
    ("000:000:000:0.0.0.1,".RegexIPv6().PassFalse()).Dump(); //Failing, A is not allowed
    (":::1,".RegexIPv6().PassFalse()).Dump(); //Failing, , is not allowed
    "����������������������".Dump();

    d.DumpAsJson("DumpAsJson");
    Console.WriteLine($"ToJson:\n{d.ToJson()}");
    "����������������������".Dump();

    var json = @"{""hopefullyValidJson"":true}";
    using (var sr = new MemoryStream(Encoding.UTF8.GetBytes(json)))
    {
        using (var j = sr.GetJsonReader())
        {
            var x = ConvenienceMethods.CreateJsonSerializer().Deserialize(j);
            x.Dump("GetJsonReader demo");
        }
    }
    "����������������������".Dump();

    @"{ i : 1 }".DeserializeJsonString<testClass>().Dump("DeserializeJsonString demo");
    DateTime.Now.ToJsonDate().Dump("local date");
    DateTime.UtcNow.ToJsonDate().Dump("utc date");
    "x".OnNotEmptyString(x => x.Dump("onNotEmptyString with value"));
    "".OnNotEmptyString(x => x.Dump("onNotEmptyString with empty"));
    new[] { "", "1" }.Coalesce().Dump("coalesce");
    "".Coalesce("", "", "2").Dump("coalesce 2");
    "123456789123456789".Truncate(5).Dump("truncate after 5");
    new[] { "stuff", "", "is", "here", "as", "", "well" }.JoinWhereNotEmpty("|").Dump("join where not empty");
    new[] { "1", "3", "5", "x", "y", "7" }.JoinWhere("|", x => int.TryParse(x, out var _)).Dump("join where parsable from int");
    DateTime.Now.ToIsoString().Dump("ToIsoString");
    //	((IEnumerable<string>)null).ListOrNull<string>("ListOrNull");
    new[] { "1", "3", "5", "x", "y", null, "7" }
    .ForEach(x =>
    {
        Console.WriteLine("a:{0}", x);
    });
    var testArr = Enumerable.Range(1, 20).ToArray();
    var (i, k) = testArr;
    var (s1, s2, s3) = testArr;
    var (t1, t2, t3, t4) = testArr;
    var (x1, x2, x3, _, x4) = testArr;
    new[] {
        i,k,s1,s2,s3,t1,t2,t3,t4,x1,x2,x3,x4
    }.Dump("Deconstructed");
    try
    {
        var t = Task.FromException(new ArgumentException("test"));
        t.Wait();
    }
    catch (AggregateException ex)
    {
        ex.ToEnumerable().ToArray().Dump("exception ToEnumerable");
    }
    using (var dt = d.Elements().CopyToDataTable())
    {
        dt.Dump("copy to data table");
    }
    using (var dt2 = new DataTable())
    {
        dt2.Columns.Add(new DataColumn("i", typeof(int)));
        new[] { new testClass { i = 1 }, new testClass { i = 5 } }.CopyToDataTable(dt2);
        dt2.Dump("Copy to datatable with existing datatable defined");
    }
    "����������������������".Dump();

    ConvenienceMethods.FolderPath(System.Environment.SpecialFolder.CommonMusic, "testfilename").Dump("FolderPath");
    ConvenienceMethods.DesktopFolderPath("testfile").Dump("Desktop path");
    "����������������������".Dump();

    new testClass() { i = 1 }.GetPropertyValues().Dump("GetPropertyValues demo");
    var tcwf = new testClassWithFields();
    tcwf.i = 1;
    tcwf.j = 2;
    // reminder, it's an enumerable so its lazy evaluated.
    tcwf.GetFieldValues((x, y, z) => $"{x} - value {y} - type {z.Name}".Dump("GetFieldValues")).ToList();
    "����������������������".Dump();

    Enumerable.Range(1, 50).Chunk(10).Dump("chunk demo");
    Enumerable.Range(1, 10).ToPrintableList().Dump("ToPrintableList demo");
*/