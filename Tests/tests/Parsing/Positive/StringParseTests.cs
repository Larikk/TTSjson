namespace Tests.Tests.Parsing.Positive;

public class StringParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();


    // General tests

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
    public void ShouldParseStringWithJustSpace()
    {
        var actual = ttsjson.Parse(Q(" ")).String;
        Assert.Equal(" ", actual);
    }

    [Fact]
    public void ShouldParseEscapedQuote()
    {
        var actual = ttsjson.Parse(Q(@"\""")).String;
        Assert.Equal("\"", actual);
    }

    [Fact]
    public void ShouldParseDoubleEscape()
    {
        var actual = ttsjson.Parse(Q("\\\\n")).String;
        Assert.Equal("\\n", actual);
    }

    [Fact]
    public void ShouldParseAllowedEscapes()
    {
        var actual = ttsjson.Parse(Q(@" \"" \\ \/ \b \f \n \r \t ")).String;
        Assert.Equal(" \" \\ / \b \f \n \r \t ", actual);


        actual = ttsjson.Parse(Q(@"\""\\\/\b\f\n\r\t")).String;
        Assert.Equal("\"\\/\b\f\n\r\t", actual);
    }

    // Unicode Literals

    [Fact]
    public void ShouldDecodeUnicodeLiteral()
    {
        var actual = ttsjson.Parse(Q("\\u20AC")).String;
        Assert.Equal("€", actual);
    }

    [Fact]
    public void ShouldDecodeMultipleUnicodeLiterals()
    {
        var actual = ttsjson.Parse(Q("\u0060\u012a\u12AB")).String;
        Assert.Equal("`Īካ", actual);
    }

    [Fact]
    public void ShouldDecodeOneByteUnicodeLiteral()
    {
        var actual = ttsjson.Parse(Q("\\u002c")).String;
        Assert.Equal("\u002c", actual);
    }

    [Fact]
    public void ShouldDecodeTwoByteUnicodeLiteral()
    {
        var actual = ttsjson.Parse(Q("\\u0123")).String;
        Assert.Equal("\u0123", actual);
    }

    [Fact]
    public void ShouldDecodeThreeByteUnicodeLiteral()
    {
        var actual = ttsjson.Parse(Q("\\u0821")).String;
        Assert.Equal("\u0821", actual);
    }

    [Fact]
    public void ShouldDecodeAcceptedSurrogatePair()
    {
        // Those may not be displayed correctly in TTS
        var actual = ttsjson.Parse(Q("\uD801\udc37")).String;
        Assert.Equal("𐐷", actual);


        actual = ttsjson.Parse(Q("\ud83d\ude39\ud83d\udc8d")).String;
        Assert.Equal("😹💍", actual);
    }

    [Fact]
    public void ShouldDecodeBackslashAndUnicodeEscapedZero()
    {
        var actual = ttsjson.Parse(Q(@"\\u0000")).String;
        Assert.Equal("\\u0000", actual);
    }

    [Fact]
    public void ShouldDecodeEscapedControlCharacter()
    {
        var actual = ttsjson.Parse(Q(@"\u0012")).String;
        Assert.Equal("\x0012", actual);
    }

    [Fact]
    public void ShouldDecodeEscapedNonCharacter()
    {
        var actual = ttsjson.Parse(Q(@"\uFFFF")).String;
        Assert.Equal("\xFFFF", actual);
    }

    [Fact]
    public void ShouldDecodeEscapedNewline()
    {
        var actual = ttsjson.Parse(Q(@"new\u000Aline")).String;
        Assert.Equal("new\nline", actual);
    }

    // Non-ASCII Bytes

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

    [Fact]
    public void ShouldDecodeMultipleUnicodeCharacters()
    {
        var actual = ttsjson.Parse(Q("\xE2\x9C\xAA\xE2\x9C\xBF\xF0\xB0\xBD\x84")).String;
        Assert.Equal("✪✿�", actual);
    }

    [Fact]
    public void ShouldDecodeMultipleUnicodeCharactersWithText()
    {
        var actual = ttsjson.Parse(Q("Lorem\xE2\x9C\xAAIpsum\xE2\x9C\xBFLorem\xF0\xB0\xBD\x84Ipsum")).String;
        Assert.Equal("Lorem✪Ipsum✿Lorem�Ipsum", actual);
    }

    // Misc

    [Fact]
    public void ShouldParseUnescapedDeleteChar()
    {
        var actual = ttsjson.Parse(Q("\x7F")).String;
        Assert.Equal("\x7F", actual);
    }

    private static string Q(string s)
    {
        return '"' + s + '"';
    }
}
