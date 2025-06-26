using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

class TTSjsonDebugServer
{
    private readonly Closure ParseFunction;

    public TTSjsonDebugServer()
    {
        int port = 41912;
        var server = new MoonSharpVsCodeDebugServer(port);

        string scriptPath = Environment.GetEnvironmentVariable("TTSJSON_PATH") ?? throw new Exception("TTSJSON_PATH is not set");
        string scriptCode = File.ReadAllText(scriptPath);
        var script = new Script();

        DynValue executionResult = script.DoString(scriptCode, null, scriptPath);
        server.AttachToScript(script, "TTSjson");
        ParseFunction = executionResult.Table.Get("parse").Function;

        server.Start();
        Thread.Sleep(2000); // Give debugger time to attach
    }

    public void DebugParsing(string json)
    {
        DynValue res = ParseFunction.Call(json);
        Console.WriteLine(res);
    }
}
