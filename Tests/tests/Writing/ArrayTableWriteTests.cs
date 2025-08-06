namespace Tests.Tests.Writing;

public class ArrayTableWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteEmptyTableAsArray()
    {
        ttsjson.EvalWrite("{}").ShouldBeEquivalentToJson("[]");
    }

    [Fact]
    public void ShouldWriteTableArrayWithOneElement()
    {
        ttsjson.EvalWrite("{true}").ShouldBeEquivalentToJson("[true]");
    }

    [Fact]
    public void ShouldWriteTableArrayWithTwoElements()
    {
        ttsjson.EvalWrite("{true, true}").ShouldBeEquivalentToJson("[true, true]");
    }

    [Fact]
    public void ShouldWriteTableArrayWithThreeElements()
    {
        ttsjson.EvalWrite("{true, true, true}").ShouldBeEquivalentToJson("[true, true, true]");
    }

    [Fact]
    public void ShouldWriteTableArrayWithGaps()
    {
        ttsjson.EvalWrite("{[1] = true, [4] = true}").ShouldBeEquivalentToJson("[true, null, null, true]");
    }
    

    [Fact]
    public void ShouldWriteNestedTableArraya()
    {
        ttsjson.EvalWrite("{{{true}}, {true}}").ShouldBeEquivalentToJson("[[[true]], [true]]");
    }

    [Fact]
    public void ShouldWriteTableArrayWithMixedTypes()
    {
        ttsjson.EvalWrite(""" {1, "value", true, nil, {key = "value"}, {true} } """)
        .ShouldBeEquivalentToJson(""" [1, "value", true, null, {"key": "value"}, [true]] """);
    }

}
