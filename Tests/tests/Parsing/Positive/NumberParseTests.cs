using MoonSharp.Interpreter;

namespace Tests.Tests.Parsing.Positive;

public class NumberParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldParseInteger()
    {
        var actual = ttsjson.Parse("100").Number;
        Assert.Equal(100, actual);
    }

    [Fact]
    public void ShouldParseDouble()
    {
        var actual = ttsjson.Parse("10.5").Number;
        Assert.Equal(10.5, actual);
    }

    [Fact]
    public void ShouldParse0ePlus1()
    {
        var actual = ttsjson.Parse("0e+1").Number;
        Assert.Equal(0e+1, actual);
    }

    [Fact]
    public void ShouldParse0e1()
    {
        var actual = ttsjson.Parse("0e1").Number;
        Assert.Equal(0e1, actual);
    }

    [Fact]
    public void ShouldParseNumberCloseToZero()
    {
        var actual = ttsjson.Parse("-0.000000000000000000000000000000000000000000000000000000000000000000000000000001").Number;
        Assert.Equal(-0.000000000000000000000000000000000000000000000000000000000000000000000000000001, actual);
    }

    [Fact]
    public void ShouldParseIntWithExp()
    {
        var actual = ttsjson.Parse("20e1").Number;
        Assert.Equal(20e1, actual);
    }

    [Fact]
    public void ShouldParseMinusZero()
    {
        var actual = ttsjson.Parse("-0").Number;
        Assert.Equal(-0, actual);
    }

    [Fact]
    public void ShouldParseNegativeInt()
    {
        var actual = ttsjson.Parse("-123").Number;
        Assert.Equal(-123, actual);
    }

    [Fact]
    public void ShouldParseRealCapitalENegExp()
    {
        var actual = ttsjson.Parse("1E-2").Number;
        Assert.Equal(1E-2, actual);
    }

    [Fact]
    public void ShouldParseRealCapitalEPosExp()
    {
        var actual = ttsjson.Parse("1E+2").Number;
        Assert.Equal(1E+2, actual);
    }

    [Fact]
    public void ShouldParseRealCapitalE()
    {
        var actual = ttsjson.Parse("1E22").Number;
        Assert.Equal(1E22, actual);
    }

    [Fact]
    public void ShouldParseRealExponent()
    {
        var actual = ttsjson.Parse("123e45").Number;
        Assert.Equal(123e45, actual);
    }

    [Fact]
    public void ShouldParseRealFractionExponent()
    {
        var actual = ttsjson.Parse("123.456e78").Number;
        Assert.Equal(123.456e78, actual);
    }

    [Fact]
    public void ShouldParseRealNegExp()
    {
        var actual = ttsjson.Parse("1e-2").Number;
        Assert.Equal(1e-2, actual);
    }

    [Fact]
    public void ShouldParseRealPosExp()
    {
        var actual = ttsjson.Parse("1e+2").Number;
        Assert.Equal(1e+2, actual);
    }
}
