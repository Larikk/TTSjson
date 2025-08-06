using MoonSharp.Interpreter;

class TTSNativeParserExecutor : IParserExecutor
{
    private readonly Closure ParseFunction;
    private readonly Closure WriteFunction;

    public TTSNativeParserExecutor()
    {
        string scriptCode = File.ReadAllText("./Benchmark/TTSNativeJSON.ttslua");
        var script = new Script();

        DynValue executionResult = script.DoString(scriptCode, null, "TTSNativeJSON");
        ParseFunction = executionResult.Table.MetaTable.Get("decode").Function;
        WriteFunction = executionResult.Table.MetaTable.Get("encode").Function;
    }

    public string GetName()
    {
        return "TTSNativeJSON";
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
