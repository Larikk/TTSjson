using MoonSharp.Interpreter;

namespace Tests.tests.parsing;

public class ObjectParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldParseEmptyObject()
    {
        var actual = ttsjson.Parse("""{}""").Table;
        Assert.Empty(actual.Keys.ToList());
    }

    [Fact]
    public void ShouldParseObjectWithOneValue()
    {
        var actual = ttsjson.Parse("""{"foo":"bar"}""").Table;
        Assert.Single(actual.Keys.ToList());
        Assert.Equal("bar", actual["foo"]);
    }

    [Fact]
    public void ShouldParseObjectWithMultipleValues()
    {
        var actual = ttsjson.Parse("""{"foo":"bar","i":5,"bool":false,"items":[]}""").Table;
        Assert.Equal(4, actual.Keys.ToList().Count);
        Assert.Equal("bar", actual["foo"]);
        Assert.Equal(5.0, actual["i"]);
        Assert.Equal(false, actual["bool"]);
        Assert.Equal([], actual.Get("items").Table.Values);
    }

    [Fact]
    public void ShouldParseDuplicatedKeyAndValue()
    {
        var actual = ttsjson.Parse("""{"a":"b","a":"b"}""").Table;
        Assert.Single(actual.Keys);
        Assert.Equal("b", actual.Get("a").String);
    }

    [Fact]
    public void ShouldParseDuplicatedKey()
    {
        var actual = ttsjson.Parse("""{"a":"b","a":"c"}""").Table;
        Assert.Single(actual.Keys);
        Assert.Equal("c", actual.Get("a").String);
    }

    [Fact]
    public void ShouldParseEmptyKey()
    {
        var actual = ttsjson.Parse("""{"":0}""").Table;
        Assert.Single(actual.Keys);
        Assert.Equal(0, actual.Get("a").Number);
    }

    [Fact]
    public void ShouldParseEscapedNullInKey()
    {
        var actual = ttsjson.Parse("""{"foo\u0000bar": 42}""").Table;
        Assert.Single(actual.Keys);
        Assert.Equal(42, actual.Get("foo\u0000bar").Number);
    }

    [Fact]
    public void ShouldParseExtremeNumbers()
    {
        var actual = ttsjson.Parse("""{ "min": -1.0e+28, "max": 1.0e+28 }""").Table;
        Assert.Equal(2, actual.Keys.Count());
        Assert.Equal(-1.0e+28, actual.Get("min").Number);
        Assert.Equal(1.0e+28, actual.Get("max").Number);
    }
}
