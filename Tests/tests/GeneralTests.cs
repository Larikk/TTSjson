using FluentAssertions;

namespace Tests.Tests;

public class GeneralTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldParseEscapedNonAsciiCharacterThenWriteItThenParseItAgain()
    {
        var sourceString = "\"Hello World + \\uD801\\uDC37\"";
        var firstParsed = ttsjson.Parse(sourceString).String;
        firstParsed.Should().Be("Hello World + 𐐷");

        var writtenString = ttsjson.Write(firstParsed);
        writtenString.ShouldBeEquivalentToJson("\"Hello World + 𐐷\"");

        var secondParsed = ttsjson.Parse(writtenString).String;
        secondParsed.Should().Be("Hello World + 𐐷");
    }
}
