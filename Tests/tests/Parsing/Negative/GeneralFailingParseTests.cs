namespace Tests.Tests.Parsing.Negative;

public class GeneralFailingParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldFailOnIncompleteNull()
    {
        var json = "nul";
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnIncompleteFalse()
    {
        var json = "fale";
        var expectedErrorMessage = "json is not terminated properly";
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

}
