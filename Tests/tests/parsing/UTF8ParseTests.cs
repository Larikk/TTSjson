namespace Tests.tests.parsing;

public class UTF8ParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

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
