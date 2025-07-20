using MoonSharp.Interpreter;

var debugServer = new TTSjsonDebugServer();
// debugServer.DebugParsing("true");

// It may be necessary to press "Step over" in the debugging UI a couple of times to step into TTSjson.lua
var table = """
    {key = "value"}
""";
debugServer.DebugEvalWriting(table);
