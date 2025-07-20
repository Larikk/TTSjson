using MoonSharp.Interpreter;

var debugServer = new TTSjsonDebugServer();
// debugServer.DebugParsing("true");

var table = new Table(null);
table["key"] = "string";
debugServer.DebugWriting(DynValue.NewTable(table));

