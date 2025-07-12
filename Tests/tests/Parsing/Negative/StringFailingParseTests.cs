namespace Tests.Tests.Parsing.Negative;

public class StringFailingParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();


    [Fact]
    public void ShouldFailOnNoQuotesWithBadEscape()
    {
        var json = "\\n";
        var expectedErrorMessage = "expected start of a value, got \\";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSingleDoublequote()
    {
        var json = "\"";
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSingleQuotes()
    {
        var json = "'single quote'";
        var expectedErrorMessage = "expected start of a value, got '";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnStringWithNoDoublequotes()
    {
        var json = "abc";
        var expectedErrorMessage = "expected start of a value, got a";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnStartEscapeUnclosed()
    {
        var json = "\"\\";
        var expectedErrorMessage = "unsupported escaped symbol ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnescapedControlChar()
    {
        var json = "a\x00a";
        var expectedErrorMessage = "expected start of a value, got a";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnescapedNewline()
    {
        var json = Q("new\nline");
        var expectedErrorMessage = "unescaped control character encountered: 0x0A";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnscapedTab()
    {
        var json = Q("\t");
        var expectedErrorMessage = "unescaped control character encountered: 0x09";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnicodeCapitalU()
    {
        var json = Q("\\UA66D");
        var expectedErrorMessage = "unsupported escaped symbol U";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnStringWithTrailingGarbage()
    {
        var json = "\"\"x";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSurrogateThenEscapeU()
    {
        var json = Q("\\uD800\\u");
        var expectedErrorMessage = "invalid unicode escape sequence: \\u\"";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSurrogateThenEscapeU1()
    {
        var json = Q("\\uD800\\u1");
        var expectedErrorMessage = "invalid unicode escape sequence: \\u1\"";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSurrogateThenEscapeU1x()
    {
        var json = Q("\\uD800\\u1x");
        var expectedErrorMessage = "invalid unicode escape sequence: \\u1x\"";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSurrogateThenEscape()
    {
        var json = Q("\\uD800\\");
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnAccentedCharacterWihoutQuotes()
    {
        var json = "é";
        var expectedErrorMessage = "expected start of a value, got é";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnquotedUnicodeSequence()
    {
        var json = "\xFFFD";
        var expectedErrorMessage = "expected start of a value, got \xFFFD";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnBackslash00()
    {
        var json = Q("\\\x00");
        var expectedErrorMessage = "unsupported escaped symbol \0";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnEscapeX()
    {
        var json = Q("\\x00");
        var expectedErrorMessage = "unsupported escaped symbol x";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnEscapedBackslashBad()
    {
        var json = Q("\\\\\\");
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnEscapedCtrlCharTab()
    {
        var json = Q("\\	");
        var expectedErrorMessage = "unsupported escaped symbol \t";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnIncompleteEscape()
    {
        var json = Q("\\");
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnIncompleteEscapedCharacter()
    {
        var json = Q("\\u00A");
        var expectedErrorMessage = "Additional non-parsable characters are at the end of the string.";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnIncompleteSurrogateEscapeInvalid()
    {
        var json = Q("\\uD800\\uD800\\x");
        var expectedErrorMessage = "unsupported escaped symbol x";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnIncompleteSurrogate()
    {
        var json = Q("\\uD834\\Dd");
        var expectedErrorMessage = "unsupported escaped symbol D";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidBackslashEsc()
    {
        var json = Q("\\a");
        var expectedErrorMessage = "unsupported escaped symbol a";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidUnicodeEscape()
    {
        var json = Q("\\uqqqq");
        var expectedErrorMessage = "Could not find any recognizable digits.";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidUtf8AfterEscape()
    {
        var json = Q("\\\xFFFD");
        var expectedErrorMessage = "unsupported escaped symbol �";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidUtf8InEscape()
    {
        var json = Q("\\u\xFFFD");
        var expectedErrorMessage = "invalid unicode escape sequence: \\u�\"";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnLeadingUnescapedThinspace()
    {
        var json = "\\u0020\"asd\"";
        var expectedErrorMessage = "expected start of a value, got \\";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    private static string Q(string s)
    {
        return '"' + s + '"';
    }
}
