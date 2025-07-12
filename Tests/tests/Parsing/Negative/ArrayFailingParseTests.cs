namespace Tests.Tests.Parsing.Negative;

public class ArrayFailingParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldFailOnNoComma()
    {
        var json = "[1 true]";
        var expectedErrorMessage = "expected ',' or ']' after array value, got 't'";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidArrayValue()
    {
        var json = "[abc]";
        var expectedErrorMessage = "expected start of a value, got 'a'";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnInvalidSeparator()
    {
        var json = "[1; true]";
        var expectedErrorMessage = "expected ',' or ']' after array value, got ';'";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnCommaAfterClose()
    {
        var json = "[1],";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnCommaBeforeFirstValue()
    {
        var json = "[,1]";
        var expectedErrorMessage = "expected start of a value, got ','";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnTrailingComma()
    {
        var json = "[1,]";
        var expectedErrorMessage = "expected start of a value, got ']'";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnDuplicateCommasBetweenValues()
    {
        var json = "[1,,2]";
        var expectedErrorMessage = "expected start of a value, got ','";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnDuplicateCommasAfterLastValue()
    {
        var json = "[1,,]";
        var expectedErrorMessage = "expected start of a value, got ','";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnExtraClose()
    {
        var json = "[1]]";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnclosedArray()
    {
        var json = "[1, 2";
        var expectedErrorMessage = "expected ',' or ']' after array value, got ''";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnMissingCommaBeforeInnerArray()
    {
        var json = "[1 [2]]";
        var expectedErrorMessage = "expected ',' or ']' after array value, got '['";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnCommaWithoutValues()
    {
        var json = "[,]";
        var expectedErrorMessage = "expected start of a value, got ','";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnJustMinus()
    {
        var json = "[-]";
        var expectedErrorMessage = "not a number: '-'";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnTrailingCommaOnSeparateLine()
    {
        var json = @"[
        1,
        2,
        3,
        ]";
        var expectedErrorMessage = "expected start of a value, got ']'";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnclosedArrayOnSeparateLine()
    {
        var json = @"[
        1,
        2,
        3";
        var expectedErrorMessage = "expected ',' or ']' after array value, got ''";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnclosedArrayWithObject()
    {
        var json = "[{}";
        var expectedErrorMessage = "expected ',' or ']' after array value, got ''";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnopenedArrayWithValue()
    {
        var json = "1]";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnUnopenedArrayWithoutValue()
    {
        var json = "]";
        var expectedErrorMessage = "expected start of a value, got ']'";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }

    [Fact]
    public void ShouldFailOnDoubleArray()
    {
        var json = "[][]";
        var expectedErrorMessage = "json has data past the parsed value";
        ttsjson.AssertFailingParse(json, expectedErrorMessage);
    }
}
