using MoonSharp.Interpreter;

interface IParserExecutor
{
    string GetName();
    DynValue Parse(string json);
    string Write(DynValue value);
}
