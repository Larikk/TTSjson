using MoonSharp.Interpreter;

namespace Tests.tests.parsing;

public class GeneralParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

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
}
