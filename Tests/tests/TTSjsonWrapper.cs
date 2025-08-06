using MoonSharp.Interpreter;

namespace Tests.Tests;

public sealed class TTSjsonWrapper
{

    private readonly Script script;
    private readonly Closure parseFunction;
    private readonly Closure writeFunction;

    public TTSjsonWrapper()
    {
        // Tests always use the location of the dll as working dir
        var workingDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        var projectDirectory = workingDirectory?.Parent?.Parent?.Parent?.Parent?.FullName;
        var ttsJsonFilePath = projectDirectory + "/TTSjson.lua";

        string scriptCode = File.ReadAllText(ttsJsonFilePath);

        script = new();
        Table ttsJsonModule = script.DoString(scriptCode).Table;
        parseFunction = ttsJsonModule.Get("parse").Function;
        writeFunction = ttsJsonModule.Get("write").Function;
    }

    public DynValue Parse(string json)
    {
        return Execute(() => parseFunction.Call(json));
    }

    public void AssertFailingParse(string json, string expectedErrorMessage)
    {
        var exception = Assert.ThrowsAny<Exception>(() => Parse(json));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }

    public string Write(DynValue value)
    {
        return Execute(() => writeFunction.Call(value).String);
    }

    public string Write(string value)
    {
        return Write(DynValue.NewString(value));
    }

    public string EvalWrite(string luaCodeForValue)
    {
        var value = script.DoString("return " + luaCodeForValue);
        return Write(value);
    }

    public void AssertFailingWrite(DynValue value, string expectedErrorMessage)
    {
        var exception = Assert.ThrowsAny<Exception>(() => Write(value));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }

    public void AssertFailingEvalWrite(string luaCodeForValue, string expectedErrorMessage)
    {
        var exception = Assert.ThrowsAny<Exception>(() => EvalWrite(luaCodeForValue));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }

    private static T Execute<T>(Func<T> func)
    {
        // Wrap execution in task and abort if task takes too long as a protection against infinite loops in the TTSjson lib
        var ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)).Token;
        Task<T> task = Task.Run(func);
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

}
