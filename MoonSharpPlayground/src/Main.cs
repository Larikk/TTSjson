using MoonSharp.Interpreter;

string scriptCode = """
print("Hello")
return "World"
""";

var script = new Script();
script.Options.DebugPrint = Console.WriteLine;
var res = script.DoString(scriptCode);
Console.WriteLine(res);
