using MoonSharp.Interpreter;

namespace Tests.Tests.Writing;

public class ErorWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldFailWhenDirectCycleDetected()
    {
        var table = new Table(null);
        table["value"] = table;
        var value = DynValue.NewTable(table);

        ttsjson.AssertFailingWrite(value, "detected a cycle between tables, aborting serialization");
    }

    [Fact]
    public void ShouldFailWhenIndirectCycleDetected()
    {
        var table1 = new Table(null);
        var table2 = new Table(null);
        table1["value"] = table2;
        table2[1] = table1;
        var value = DynValue.NewTable(table1);

        ttsjson.AssertFailingWrite(value, "detected a cycle between tables, aborting serialization");
    }

    [Fact]
    public void ShouldFailOnNan()
    {
        ttsjson.AssertFailingEvalWrite("0/0", "encountered NaN during serialization");
    }
}