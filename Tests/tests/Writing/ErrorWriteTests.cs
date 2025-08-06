using MoonSharp.Interpreter;

namespace Tests.Tests.Writing;

public class ErorWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();
    private class UserdataClass { };

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
    public void ShouldFailWhenStringKeyComesAfterNumericalKey()
    {
        ttsjson.AssertFailingEvalWrite("""
            {[1] = 1, ["key"] = "value"}
        """, "encountered non-numerical key in array-like table: 'key' with type 'string'");
    }

    [Fact]
    public void ShouldFailWhenNumericalKeyComesAfterStringKey()
    {
        ttsjson.AssertFailingEvalWrite("""
            {["key"] = "value", [1] = 1}
        """, "encountered non-string key in object-like table: '1' with type 'number'");
    }

    [Fact]
    public void ShouldFailWhenKeysAreNeitherNumericalOrString()
    {
        ttsjson.AssertFailingEvalWrite("""
            {[true] = true}
        """, "encountered unsupported key type: 'true' with type 'boolean'");
    }

    [Fact]
    public void ShouldFailOnNan()
    {
        ttsjson.AssertFailingEvalWrite("0/0", "encountered NaN during serialization");
    }

    [Fact]
    public void ShouldFailOnUserdata()
    {
        UserData.RegisterType<UserdataClass>();
        ttsjson.AssertFailingWrite(UserData.Create(new UserdataClass()), "unsupported value type 'userdata'");
    }

    [Fact]
    public void ShouldFailOnFunction()
    {
        ttsjson.AssertFailingEvalWrite("function() end", "unsupported value type 'function'");
    }
}
