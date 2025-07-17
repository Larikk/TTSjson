using FluentAssertions;

namespace Tests.Tests.Writing;

public class ObjectTableWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteObjectTable()
    {
        var actual = ttsjson.EvalWrite(@"{key = ""value""}");
        var expected = @"{""key"":""value""}";
        actual.Should().BeEquivalentTo(expected);
    }

}