using MoonSharp.Interpreter;


static void test()
{
    string scriptCode = """
    print("Hello")
    return "World"
    """;

    var script = new Script();
    script.Options.DebugPrint = Console.WriteLine;
    var res = script.DoString(scriptCode);
    Console.WriteLine(res);
}

Task task = Task.Run(() => test());
var ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)).Token;
await task.WaitAsync(ct);
