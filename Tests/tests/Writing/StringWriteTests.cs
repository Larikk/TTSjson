using FluentAssertions;

namespace Tests.Tests.Writing;

public class StringWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteString()
    {
        var actual = ttsjson.EvalWrite(@"""foo""");
        var expected = @"""foo""";
        actual.Should().BeEquivalentTo(expected);
    }

}