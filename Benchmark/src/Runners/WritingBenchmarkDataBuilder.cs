using MoonSharp.Interpreter;

public class WritingBenchmarkDataBuilder
{

    private static DynValue CopyTable(DynValue sourceValue)
    {
        var resultValue = DynValue.NewPrimeTable();
        var resultTable = resultValue.Table;

        var sourceTable = sourceValue.Table;
        var keys = sourceTable.Keys;
        foreach (var key in keys)
        {
            resultTable[key] = CopyValue(sourceTable.Get(key));
        }

        return resultValue;
    }

    private static DynValue CopyValue(DynValue value)
    {
        return value.Type switch
        {
            DataType.Boolean or DataType.Nil or DataType.Number or DataType.String => value,
            DataType.Table => CopyTable(value),
            _ => throw new Exception("Unsupported type " + value.Type),
        };
    }

    public static DynValue BuildBenchmarkData(DynValue sourceTable)
    {
        return CopyValue(sourceTable);
    }
}