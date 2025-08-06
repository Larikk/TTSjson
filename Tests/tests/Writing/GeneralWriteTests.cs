namespace Tests.Tests.Writing;

public class GeneralWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteTrue()
    {
        ttsjson.EvalWrite("true")
            .ShouldBeEquivalentToJson("true");
    }

    [Fact]
    public void ShouldWriteFalse()
    {
        ttsjson.EvalWrite("false")
            .ShouldBeEquivalentToJson("false");
    }

    [Fact]
    public void ShouldWriteNil()
    {
        ttsjson.EvalWrite("nil")
            .ShouldBeEquivalentToJson("null");
    }
}