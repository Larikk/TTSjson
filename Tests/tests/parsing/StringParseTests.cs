namespace Tests.tests.parsing;

public class StringParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();


    [Fact]
    public void ShouldParseString()
    {
        var actual = ttsjson.Parse(Q("Foo")).String;
        Assert.Equal("Foo", actual);
    }

    [Fact]
    public void ShouldParseLongString()
    {
        var actual = ttsjson.Parse(Q("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")).String;
        Assert.Equal("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", actual);
    }

    [Fact]
    public void ShouldDecodeUnicodeLiteral()
    {
        var actual = ttsjson.Parse(Q("\\u20AC")).String;
        Assert.Equal("€", actual);
    }

    [Fact]
    public void ShouldDecodeEuroSign()
    {
        var actual = ttsjson.Parse(Q("\xE2\x82\xAC")).String;
        Assert.Equal("€", actual);
    }

    [Fact]
    public void ShouldDecodeHalfWhiteCircle()
    {
        var actual = ttsjson.Parse(Q("\xEF\xBF\xAE")).String;
        Assert.Equal("￮", actual);
    }

    [Fact]
    public void ShouldDecodeUnicodeCharactersAboveFFFFasFFFD()
    {
        var actual = ttsjson.Parse(Q("\xF0\x90\x80\x80")).String;
        Assert.Equal("�", actual);
    }

    private static string Q(string s)
    {
        return '"' + s + '"';
    }
}
