using MoonSharp.Interpreter;

namespace Tests.tests;

public class PositiveParseTests
{
    private static readonly TTSjsonWrapper ttsjson = TTSjsonWrapper.Instance;

    // 
    // Simple Tests
    //
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

    //
    // Whitespace tests
    //
    [Fact]
    public void ShouldIgnoreWhitespaceBetweekTokens()
    {
        var json =
"""   

                  {  
        "foo"      :  "bar"       ,    
        
        "i":5 ,  "bool"       :
        false
    ,
    "items"  :        [   "foo"   ,
    
    "bar"     ,
"baz",
 ]
 
             }
""";
        var actual = ttsjson.Parse(json).Table;
        Assert.Equal(4, actual.Keys.ToList().Count);
        Assert.Equal("bar", actual["foo"]);
        Assert.Equal(5.0, actual["i"]);
        Assert.Equal(false, actual["bool"]);
        Assert.Equal(["foo", "bar", "baz"], ((Table)actual["items"]).AsList(i => i.String));
    }

    [Fact]
    public void ShouldIgnoreLeadingAndTrailingWhitespace()
    {
        var actual = ttsjson.Parse("     \n  \t   \"Foo\"    \n \t   ").String;
        Assert.Equal("Foo", actual);
    }


    //
    // UTF-8 Tests
    //
    [Fact]
    public void ShouldDecodeUnicodeLiteral()
    {
        var actual = ttsjson.Parse("\"\\u20AC\"").String;
        Assert.Equal("€", actual);
    }

    [Fact]
    public void ShouldDecodeEuroSign()
    {
        var actual = ttsjson.Parse("\"\xE2\x82\xAC\"").String;
        Assert.Equal("€", actual);
    }

    [Fact]
    public void ShouldDecodeHalfWhiteCircle()
    {
        var actual = ttsjson.Parse("\"\xEF\xBF\xAE\"").String;
        Assert.Equal("￮", actual);
    }

    [Fact]
    public void ShouldDecodeUnicodeCharactersAboveFFFFasFFFD()
    {
        var actual = ttsjson.Parse("\"\xF0\x90\x80\x80\"").String;
        Assert.Equal("�", actual);
    }
}
