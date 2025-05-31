using MoonSharp.Interpreter;

namespace Tests.tests;

public static class MyExtensions
{
    public static List<T> AsList<T>(this Table table, Func<DynValue, T> mapFunc)
    {
        return table.Values.Select(mapFunc).ToList();
    }
}
