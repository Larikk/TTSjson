namespace Tests.Tests.Parsing.Negative;

public class NumberFailingParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldFailOnNumberThen00()
    {
        var json = "123\x00";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnLeadingDot()
    {
        var json = ".-1";
        var expectedErrorMessage = "expected start of a value, got .";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSpaceBetweenDigits()
    {
        var json = "1 000.0";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Theory]
    [InlineData("+1")]
    [InlineData("++1")]
    public void ShouldFailOnLeadingPlus(string json)
    {
        var expectedErrorMessage = "expected start of a value, got +";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Theory]
    [InlineData("Nan", "expected start of a value, got N")]
    [InlineData("Inf", "expected start of a value, got I")]
    [InlineData("+Nan", "expected start of a value, got +")]
    [InlineData("+Inf", "expected start of a value, got +")]
    [InlineData("-Nan", "not a number: -")]
    [InlineData("-Inf", "not a number: -")]
    public void ShouldFailOnNonOrInf(string json, string expectedErrorMessage)
    {
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    // Commented out values are not valid json numbers but Lua parses them fine
    [Theory]
    [InlineData("-1.0.")]
    //[InlineData("-01")]
    //[InlineData("-2.")]
    [InlineData("0E")]
    [InlineData("0.1.2")]
    [InlineData("0E+")]
    [InlineData("0.3e")]
    [InlineData("0.3e+")]
    //[InlineData("0.e1")]
    [InlineData("0e")]
    [InlineData("0e+")]
    [InlineData("1.0e-")]
    [InlineData("1.0e")]
    [InlineData("1.0e+")]
    [InlineData("1eE2")]
    //[InlineData("2.e-3")]
    //[InlineData("2.e+3")]
    //[InlineData("2.e3")]
    [InlineData("9.e+")]
    public void ShouldFailOnInvalidNumbers(string json)
    {
        var expectedErrorMessage = "not a number: " + json;
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnHexDigits()
    {
        var json = "0x1";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidCharactersInNumber()
    {
        var json = "-123.456foo";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSpaceAfterMinus()
    {
        var json = "- 1";
        var expectedErrorMessage = "not a number: -";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    /* Invalid json but Lua can parse that
    [Fact]
    public void ShouldFailOnNegativeRealNumberWithoutIntPart()
    {
        var json = "-.123";
        var expectedErrorMessage = "";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }
    */

    /* Invalid json but Lua can parse that
    [Fact]
    public void ShouldFailOnRealNumberWithoutFractionalPart()
    {
        var json = "1.";
        var expectedErrorMessage = "";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }
    */

}
