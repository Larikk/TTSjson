using FluentAssertions;

namespace Tests.Tests.Writing;

public class StringWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteString()
    {
        var actual = ttsjson.Write("foo");
        var expected = Q("foo");
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteEmptyString()
    {
        var actual = ttsjson.Write("");
        var expected = Q("");
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteBlankString()
    {
        var actual = ttsjson.Write("  ");
        var expected = Q("  ");
        actual.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void ShouldEscapeNewLineInText()
    {
        var actual = ttsjson.Write("foo\nbar");
        var expected = Q("foo\\nbar");
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("\"", "\\\"")]
    [InlineData("\\", "\\\\")]
    [InlineData("\n", "\\n")]
    [InlineData("\r", "\\r")]
    [InlineData("\t", "\\t")]
    [InlineData("\b", "\\b")]
    [InlineData("\f", "\\f")]
    [InlineData("\"\\\n\r\t\b\f", "\\\"\\\\\\n\\r\\t\\b\\f")]
    public void ShouldEscapeCharacterWhenNecessary(string character, string expectedEscapedForm)
    {
        ttsjson.Write(character).Should().BeEquivalentTo(Q(expectedEscapedForm));
        ttsjson.Write(character + "foo").Should().BeEquivalentTo(Q(expectedEscapedForm + "foo"));
        ttsjson.Write("foo" + character + "bar").Should().BeEquivalentTo(Q("foo" + expectedEscapedForm + "bar"));
        ttsjson.Write("foo" + character).Should().BeEquivalentTo(Q("foo" + expectedEscapedForm));
    }

    [Fact]
    public void ShouldEscapeNonPrintableCharacters()
    {
        // 0x20 (space) and 0x7E (~) are the first and last printable ASCII characters
        ttsjson.Write("\x00\x1F\x20\x7E\x7F")
            .Should()
            .BeEquivalentTo(Q("\\u0000\\u001F ~\\u007F"));
    }

    [Theory]
    [InlineData("€")]
    [InlineData("Īካ")]
    [InlineData("𐐷")]
    [InlineData("😹💍")]
    // The following characters second byte is equal to the ASCII values of characters that needs to be escaped. The character should remain as is
    [InlineData("Ģ")] // Second byte equal to ASCII value of double quote
    [InlineData("Ŝ")] // Second byte equal to ASCII value of backslash
    [InlineData("Ċ")] // Second byte equal to ASCII value of line feed
    public void ShouldWriteNonAsciiCharacters(string character)
    {
        ttsjson.Write(character).Should().BeEquivalentTo(Q(character));
        ttsjson.Write(character + "foo").Should().BeEquivalentTo(Q(character + "foo"));
        ttsjson.Write("foo" + character + "bar").Should().BeEquivalentTo(Q("foo" + character + "bar"));
        ttsjson.Write("foo" + character).Should().BeEquivalentTo(Q("foo" + character));
    }

    private static string Q(string s)
    {
        return "\"" + s + "\"";
    }

}