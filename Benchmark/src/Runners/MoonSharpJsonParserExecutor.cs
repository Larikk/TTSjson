using MoonSharp.Interpreter;

class MoonSharpJsonParserExecutor : IParserExecutor
{
    private readonly Closure ParseFunction;

    public MoonSharpJsonParserExecutor()
    {
        var scriptCode = """
        local module = {}

        local function replaceNegativeNumbers(str)
            -- moonsharps json cant handle them
            str = string.gsub(str, [["atk":%-1]], [["atk":0]])
            str = string.gsub(str, [["def":%-1]], [["def":0]])
            return str
        end

        function convertUnicodeSequences(str)
            local pattern = [[\u[0-9a-fA-F]+]]
            local f = function(s) return string.sub(s, 1, 2) .. "{" .. string.sub(s, 3, 6) .. "}" .. string.sub(s, 7) end
            return string.gsub(str, pattern, f)
        end

        function module.decode(str)
            str = convertUnicodeSequences(str)
            str = replaceNegativeNumbers(str)
            return json.parse(str)
        end

        return module
        """;
        var script = new Script();

        DynValue executionResult = script.DoString(scriptCode);
        ParseFunction = executionResult.Table.Get("decode").Function;
    }

    public string GetName()
    {
        return "MoonSharpJson";
    }

    public void Parse(string json)
    {
        ParseFunction.Call(json);
    }
}

