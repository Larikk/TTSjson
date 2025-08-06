using FluentAssertions;
using FluentAssertions.Json;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Tests.Tests;

public static class MyExtensions
{
    public static List<T> AsList<T>(this Table table, Func<DynValue, T> mapFunc)
    {
        return table.Values.Select(mapFunc).ToList();
    }

    public static string Repeat(this string s, int times)
    {
        return string.Concat(Enumerable.Repeat(s, times));
    }

    public static AndConstraint<JTokenAssertions> ShouldBeEquivalentToJson(this string s, string json)
    {
        return JToken.Parse(s).Should().BeEquivalentTo(json);
    }
}
