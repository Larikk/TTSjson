using MoonSharp.Interpreter;

namespace Tests.tests.parsing;

public class GeneralParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldParseString()
    {
        var actual = ttsjson.Parse("\"Foo\"").String;
        Assert.Equal("Foo", actual);
    }

    [Fact]
    public void ShouldParseInteger()
    {
        var actual = ttsjson.Parse("100").Number;
        Assert.Equal(100, actual);
    }

    [Fact]
    public void ShouldParseTrue()
    {
        var actual = ttsjson.Parse("true").Boolean;
        Assert.True(actual);
    }

    [Fact]
    public void ShouldParseFalse()
    {
        var actual = ttsjson.Parse("false").Boolean;
        Assert.False(actual);
    }

    [Fact]
    public void ShouldParseNull()
    {
        var actual = ttsjson.Parse("null").IsNil();
        Assert.True(actual);
    }

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
        Assert.Equal([], ((Table)actual["items"]).Values);
    }
}
