using MoonSharp.Interpreter;


static void test()
{
    string scriptCode = """
    return "Hello World"
    """;
    var res = Script.RunString(scriptCode).String;
    Console.WriteLine(res);
}

Task task = Task.Run(() => test());
var ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)).Token;
await task.WaitAsync(ct);
