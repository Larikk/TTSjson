using MoonSharp.Interpreter;

class TTSjsonParserExecutor : IParserExecutor
{
    private readonly Closure ParseFunction;

    public TTSjsonParserExecutor()
    {
        string scriptCode = File.ReadAllText("./TTSjson.lua");
        var script = new Script();

        DynValue executionResult = script.DoString(scriptCode, null, "TTSjson");
        ParseFunction = executionResult.Table.Get("parse").Function;
    }

    public string GetName()
    {
        return "TTSjson";
    }

    public void Parse(string json)
    {
        ParseFunction.Call(json);
    }
}
