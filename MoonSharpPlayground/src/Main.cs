using MoonSharp.Interpreter;

var script = new Script();
script.Options.DebugPrint = Console.WriteLine;

var filename = "MoonSharpPlayground/Playground.lua";
script.DoFile(filename);
