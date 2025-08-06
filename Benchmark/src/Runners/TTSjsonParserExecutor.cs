using MoonSharp.Interpreter;

class TTSjsonParserExecutor : IParserExecutor
{
    private readonly Closure ParseFunction;
    private readonly Closure WriteFunction;

    public TTSjsonParserExecutor()
    {
        string scriptCode = File.ReadAllText("./TTSjson.lua");
        var script = new Script();

        DynValue executionResult = script.DoString(scriptCode, null, "TTSjson");
        ParseFunction = executionResult.Table.Get("parse").Function;
        WriteFunction = executionResult.Table.Get("write").Function;
    }

    public string GetName()
    {
        return "TTSjson";
    }

    public DynValue Parse(string json)
    {
        return ParseFunction.Call(json);
    }

    public string Write(DynValue value)
    {
        return WriteFunction.Call(value).String;
    }
}
