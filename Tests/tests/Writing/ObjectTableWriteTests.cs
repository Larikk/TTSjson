using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;

namespace Tests.Tests.Writing;

public class ObjectTableWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteObjectTable()
    {
        var actual = ttsjson.EvalWrite(@"{key = ""value""}");
        var expected = @"{""key"":""value""}";
        actual.Should().Be(expected);
    }

    [Fact]
    public void ShouldWriteNestedObjectTable()
    {
        var actual = ttsjson.EvalWrite(@"{key = {key = ""value""}}");
        var expected = @"{""key"":{""key"":""value""}}";
        actual.Should().Be(expected);
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
        JToken.Parse(actual).Should().BeEquivalentTo(JToken.Parse(expected));
    }

}