namespace Tests.Tests.Writing;

public class ObjectTableWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteObjectTableWithOneKey()
    {
        ttsjson.EvalWrite(""" {key = "value"} """)
            .ShouldBeEquivalentToJson(""" {"key":"value"}""");
    }

    [Fact]
    public void ShouldWriteObjectTableWithTwoKeys()
    {
        ttsjson.EvalWrite(""" {key1 = "value", key2 = "value"} """)
            .ShouldBeEquivalentToJson(""" {"key1":"value", "key2":"value"}""");
    }

    [Fact]
    public void ShouldWriteObjectTableWithThreeKeys()
    {
        ttsjson.EvalWrite(""" {key1 = "value", key2 = "value", key3 = "value"} """)
            .ShouldBeEquivalentToJson(""" {"key1":"value", "key2":"value", "key3":"value"}""");
    }

    [Fact]
    public void ShouldWriteNestedObjectTable()
    {
        ttsjson.EvalWrite(""" {key = {key = "value"}}""")
            .ShouldBeEquivalentToJson(""" {"key":{"key":"value"}}""");
    }

    [Fact]
    public void ShouldWriteTableWithAllPossibleValueTypesExceptNil()
    {
        var actual = ttsjson.EvalWrite("""
            {
                _nil = nil,
                _true = true,
                _false = false,
                _number = 123,
                _array = {1, 2, 3},
                _object = {key = "value"},
            }
        """);
        var expected = """
            {
                "_true": true,
                "_false": false,
                "_number": 123,
                "_array": [1, 2, 3],
                "_object": {"key": "value"}
            }
        """;
        actual.ShouldBeEquivalentToJson(expected);
    }

}
