using MoonSharp.Interpreter;

class TTSNativeParserExecutor : IParserExecutor
{
    private readonly Closure ParseFunction;

    public TTSNativeParserExecutor()
    {
        string scriptCode = File.ReadAllText("./Benchmark/TTSNativeJSON.ttslua");
        var script = new Script();

        DynValue executionResult = script.DoString(scriptCode, null, "TTSNativeJSON");
        ParseFunction = executionResult.Table.MetaTable.Get("decode").Function;
    }

    public string GetName()
    {
        return "TTSNativeJSON";
    }

    public void Parse(string json)
    {
        ParseFunction.Call(json);
    }
}
