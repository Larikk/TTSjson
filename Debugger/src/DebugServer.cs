using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

class DebugServer
{
    public static void DebugJsonString(string str)
    {
        int port = 41912;
        var server = new MoonSharpVsCodeDebugServer(port);
        
        string scriptPath = Directory.GetCurrentDirectory() + "/TTSjson.lua";
        string scriptCode = File.ReadAllText(scriptPath);
        var script = new Script();

        DynValue executionResult = script.DoString(scriptCode, null, scriptPath);
        server.AttachToScript(script, "TTSjson");
        var parseFunction = executionResult.Table.Get("parse").Function;

        server.Start();
        Thread.Sleep(2000); // Give debugger time to attach

        DynValue res = parseFunction.Call(str);
        Console.WriteLine(res);
    }
}
