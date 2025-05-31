using MoonSharp.Interpreter;

namespace Tests.tests.parsing;

public class WhitespaceParseTests
{
    private static readonly TTSjsonWrapper ttsjson = new();

    [Fact]
    public void ShouldIgnoreWhitespaceBetweekTokens()
    {
        var json =
"""   

                  {  
        "foo"      :  "bar"       ,    
        
        "i":5 ,  "bool"       :
        false
    ,
    "items"  :        [   "foo"   ,
    
    "bar"     ,
"baz",
 ]
 
             }
""";
        var actual = ttsjson.Parse(json).Table;
        Assert.Equal(4, actual.Keys.ToList().Count);
        Assert.Equal("bar", actual["foo"]);
        Assert.Equal(5.0, actual["i"]);
        Assert.Equal(false, actual["bool"]);
        Assert.Equal(["foo", "bar", "baz"], ((Table)actual["items"]).AsList(i => i.String));
    }

    [Fact]
    public void ShouldIgnoreLeadingAndTrailingWhitespace()
    {
        var actual = ttsjson.Parse("     \n  \t   \"Foo\"    \n \t   ").String;
        Assert.Equal("Foo", actual);
    }
}
