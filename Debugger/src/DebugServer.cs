using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

class DebugServer
{
    public static void DebugJsonString(string str)
    {
        int port = 41912;
        MoonSharpVsCodeDebugServer server = new(port);
        server.Start();

        string scriptPath = Directory.GetCurrentDirectory() + "/TTSjson.lua";
        string scriptCode = File.ReadAllText(scriptPath);
        Console.WriteLine(scriptCode);
        Script script = new();

        DynValue executionResult = script.DoString(scriptCode, null, scriptPath);
        server.AttachToScript(script, "TTSjson");

        var parseFunction = executionResult.Table.Get("parse").Function;

        Thread.Sleep(2000); // Give Debug Client enough time to attach before starting the parsing
        DynValue res = parseFunction.Call(str);
        Console.WriteLine(res);
    }
}
