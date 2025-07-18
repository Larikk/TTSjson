using MoonSharp.Interpreter;

namespace Tests.Tests;

public sealed class TTSjsonWrapper
{

    private readonly Closure parseFunction;

    public TTSjsonWrapper()
    {
        // Tests always use the location of the dll as working dir
        var workingDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        var projectDirectory = workingDirectory?.Parent?.Parent?.Parent?.Parent?.FullName;
        var ttsJsonFilePath = projectDirectory + "/TTSjson.lua";

        string scriptCode = File.ReadAllText(ttsJsonFilePath);
        Table ttsJsonModule = Script.RunString(scriptCode).Table;
        parseFunction = ttsJsonModule.Get("parse").Function;
    }

    public DynValue Parse(string json)
    {
        // Wrap parsing in task and abort if parsing takes too long as a protection against endless loop in the TTSjson lib
        var ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)).Token;
        Task<DynValue> task = Task.Run(() => parseFunction.Call(json));
        try
        {
            task.Wait(ct);
        }
        catch (AggregateException e)
        {
            throw e.InnerException ?? e;
        }
        return task.Result;
    }

    public void AssertFailingParse(string json, string expectedErrorMessage)
    {
        var exception = Assert.ThrowsAny<Exception>(() => Parse(json));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }

}
