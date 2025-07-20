using FluentAssertions;

namespace Tests.Tests.Writing;

public class NumberWriteTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldWriteInteger()
    {
        var actual = ttsjson.EvalWrite("1");
        var expected = "1";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteFloat()
    {
        var actual = ttsjson.EvalWrite("1.3");
        var expected = "1.3";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteNegativeInteger()
    {
        var actual = ttsjson.EvalWrite("-1");
        var expected = "-1";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteNegativeFloat()
    {
        var actual = ttsjson.EvalWrite("-1.3");
        var expected = "-1.3";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteIntegerWithE()
    {
        var actual = ttsjson.EvalWrite("5e3");
        var expected = "5000";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteFloatWithE()
    {
        var actual = ttsjson.EvalWrite("20.1e3");
        var expected = "20100";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteIntegerWithMinusE()
    {
        var actual = ttsjson.EvalWrite("5e-3");
        var expected = "0.005";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteFloatWithMinusE()
    {
        var actual = ttsjson.EvalWrite("20.1e-3");
        var expected = "0.0201";
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ShouldWriteIntegerWithCapitalE()
    {
        var actual = ttsjson.EvalWrite("5E3");
        var expected = "5000";
        actual.Should().BeEquivalentTo(expected);
    }
}