using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

class TTSjsonDebugServer
{
    private readonly Script script;
    private readonly Closure parseFunction;
    private readonly Closure writeFunction;

    public TTSjsonDebugServer()
    {
        var port = 41912;
        var server = new MoonSharpVsCodeDebugServer(port);

        var scriptPath = Environment.GetEnvironmentVariable("TTSJSON_PATH") ?? throw new Exception("TTSJSON_PATH is not set");
        var scriptCode = File.ReadAllText(scriptPath);
        script = new Script();

        DynValue executionResult = script.DoString(scriptCode, null, scriptPath);
        server.AttachToScript(script, scriptPath);
        parseFunction = executionResult.Table.Get("parse").Function;
        writeFunction = executionResult.Table.Get("write").Function;
        server.Start();
        Thread.Sleep(2000); // Give debugger time to attach
    }

    public void DebugParsing(string json)
    {
        DynValue res = parseFunction.Call(json);
        Console.WriteLine(res);
    }

    public void DebugWriting(DynValue value)
    {
        string json = writeFunction.Call(value).String;
        Console.WriteLine(json);
    }

    public void DebugEvalWriting(string luaCodeForValue)
    {
        var value = script.DoString("return " + luaCodeForValue.Trim());
        DebugWriting(value);
    }
}
