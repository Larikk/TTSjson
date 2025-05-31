using MoonSharp.Interpreter;

namespace Tests.tests.parsing;

public class ArrayParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldParseEmptyArray()
    {
        var actual = ttsjson.Parse("[]").Table.AsList(i => i.String);
        var expected = new List<string>();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ShouldParseArrayOfNumbers()
    {
        var actual = ttsjson.Parse("[1,2,3]").Table.AsList(i => (int)i.Number);
        var expected = new List<int>() { 1, 2, 3 };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ShouldParseArrayOfString()
    {
        var actual = ttsjson.Parse("""["foo","bar","baz"]""").Table.AsList(i => i.String);
        var expected = new List<string>() { "foo", "bar", "baz" };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ShouldParseArrayOfMixedTypes()
    {
        var actual = ttsjson.Parse("""[100,"foobar",true,{}]""").Table.Values.ToList();
        Assert.Equal(4, actual.Count);
        Assert.Equal(100, actual[0].Number);
        Assert.Equal("foobar", actual[1].String);
        Assert.True(actual[2].Boolean);
        Assert.Empty(actual[3].Table.Keys);
    }

    [Fact]
    public void ShouldParseArrayWithEmptyString()
    {
        var actual = ttsjson.Parse("""[""]""").Table;
        Assert.Equal(1, actual.Length);
        Assert.Equal("", actual.Get(1).String);
    }

    [Fact]
    public void ShouldParseArrayWithNull()
    {
        var actual = ttsjson.Parse("""[null]""").Table;
        Assert.Equal(0, actual.Length);
    }

    [Fact]
    public void ShouldParseArrayWithSeveralNulls()
    {
        var actual = ttsjson.Parse("""[1,null,null,null,2]""").Table;
        Assert.Equal(1, actual.Length);
        Assert.Equal(1, actual.Get(1).Number);
        Assert.Equal(2, actual.Get(5).Number);
    }
}
