using MoonSharp.Interpreter;


static void test()
{
    var c = char.ConvertFromUtf32(65536);
    string scriptCode = """
    return string.char(65536)
    """;
    var res = Script.RunString(scriptCode).String;
    Console.WriteLine(res);
}

Task task = Task.Run(() => test());
var ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)).Token;
await task.WaitAsync(ct);
