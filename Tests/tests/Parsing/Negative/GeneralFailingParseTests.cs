namespace Tests.Tests.Parsing.Negative;

public class GeneralFailingParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldFailOnIncompleteNull()
    {
        var json = "nul";
        var expectedErrorMessage = "expected null, got nul";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnIncompleteFalse()
    {
        var json = "fale";
        var expectedErrorMessage = "expected false, got fale";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnIncompleteTrue()
    {
        var json = "tr";
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnNonValue()
    {
        var json = "falsey";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnCapitalizedTrue()
    {
        var json = "True";
        var expectedErrorMessage = "expected start of a value, got T";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnEmptyString()
    {
        var json = "";
        var expectedErrorMessage = "expected start of a value, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnJustSpace()
    {
        var json = " ";
        var expectedErrorMessage = "expected start of a value, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnJustNewLine()
    {
        var json = "\n";
        var expectedErrorMessage = "expected start of a value, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnLotsOfOpeningBrackets()
    {
        var json = "[".Repeat(10_000);
        var expectedErrorMessage = "expected start of a value, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidDeeplyNestedStructture()
    {
        var json = "[{\"\":".Repeat(1_000);
        var expectedErrorMessage = "expected start of a value, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnAngleBrackets()
    {
        var json = "<.>";
        var expectedErrorMessage = "expected start of a value, got <";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnAngleBracketsNull()
    {
        var json = "<null>";
        var expectedErrorMessage = "expected start of a value, got <";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnArrayWithUnclosedString()
    {
        var json = "[\"asd]";
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenArrayOpenObject()
    {
        var json = "[{";
        var expectedErrorMessage = "expected start of string, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenArrayOpenString()
    {
        var json = "[\"a";
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenArrayString()
    {
        var json = "[\"a\"";
        var expectedErrorMessage = "expected ',' or ']' after array value but got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenObjectCloseArray()
    {
        var json = "{]";
        var expectedErrorMessage = "expected start of string, got ]";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenObjectComma()
    {
        var json = "{,";
        var expectedErrorMessage = "expected start of string, got ,";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenObjectOpenArray()
    {
        var json = "{[";
        var expectedErrorMessage = "expected start of string, got [";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenObjectOpenString()
    {
        var json = "{\"a";
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnOpenObjectStringWithApostrophes()
    {
        var json = "{'a'";
        var expectedErrorMessage = "expected start of string, got '";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnclosedArrayPartialNull()
    {
        var json = "[false, nul";
        var expectedErrorMessage = "expected null, got nul";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnclosedArrayUnfinishedNull()
    {
        var json = "[true, fals]";
        var expectedErrorMessage = "expected false, got fals]";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

}
