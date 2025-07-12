namespace Tests.Tests.Parsing.Negative;

public class ObjectFailingParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldFailOnOpenObject()
    {
        var json = """{""";
        var expectedErrorMessage = "expected start of object key, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnBracketAsKey()
    {
        var json = """{[: "x"}""";
        var expectedErrorMessage = "expected start of object key, got [";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnWrongKeyValueSeparator()
    {
        var json = """{"x", null}""";
        var expectedErrorMessage = "expected :, got ,";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnDoubleConol()
    {
        var json = """{"x"::"b"}""";
        var expectedErrorMessage = "expected start of a value, got :";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnTrailingGarbageAfterValue()
    {
        var json = """{"a":"a" 123}""";
        var expectedErrorMessage = "expected ',' or '}' after object value but got 1";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSingleQuotesInKey()
    {
        var json = """{'key':0}""";
        var expectedErrorMessage = "expected start of object key, got '";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnSingleQuotesInValue()
    {
        var json = """{"key": 'value'}""";
        var expectedErrorMessage = "expected start of a value, got '";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnMissingKey()
    {
        var json = """{:"b"}""";
        var expectedErrorMessage = "expected start of object key, got :";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnMissingColon()
    {
        var json = """{"a" "b"}""";
        var expectedErrorMessage = "expected :, got \"";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnMissingValue()
    {
        var json = """{"a":""";
        var expectedErrorMessage = "expected start of a value, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnMissingColonAndValue()
    {
        var json = "{\"a\"";
        var expectedErrorMessage = "expected :, got ";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnNonStringKey()
    {
        var json = """{1:1}""";
        var expectedErrorMessage = "expected start of object key, got 1";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnNonKeyStringButHugeNumberInstead()
    {
        var json = """{9999E9999:1}""";
        var expectedErrorMessage = "expected start of object key, got 9";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnRepeatedNulls()
    {
        var json = """{null:null,null:null}""";
        var expectedErrorMessage = "expected start of object key, got n";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnTrailingComma()
    {
        var json = """{"id":0,}""";
        var expectedErrorMessage = "expected start of object key, got }";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnMultipleTrailingCommas()
    {
        var json = """{"id":0,,,,,}""";
        var expectedErrorMessage = "expected start of object key, got ,";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnMultipleCommasInARow()
    {
        var json = """{"a":"b",,"c":"d"}""";
        var expectedErrorMessage = "expected start of object key, got ,";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnqoutedKey()
    {
        var json = """{a: "b"}""";
        var expectedErrorMessage = "expected start of object key, got a";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnterminatedValue()
    {
        var json = """{"a":"a""";
        var expectedErrorMessage = "json is not terminated properly";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnKeyWithoutValue()
    {
        var json = """{ "foo" : "bar", "a" }""";
        var expectedErrorMessage = "expected :, got }";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnObjectWithTrailingGarbage()
    {
        var json = """{"a":"b"}#""";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnExtraClose()
    {
        var json = """{}}""";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

}
