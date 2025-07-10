local module = {}

local parseObject
local parseArray

local ASCII_SPACE = 0x20
local ASCII_HORIZONTAL_TAB = 0x09  -- \t
local ASCII_LINE_FEED = 0x0A       --\n
local ASCII_CARRIAGE_RETURN = 0x0D -- \r
local ASCII_DOUBLE_QUOTE = 0x22
local ASCII_PLUS = 0x2B
local ASCII_COMMA = 0x2C
local ASCII_MINUS = 0x2D
local ASCII_DOT = 0x2E
local ASCII_FORWARDSLASH = 0x2F
local ASCII_0 = 0x30
local ASCII_1 = 0x31
local ASCII_2 = 0x32
local ASCII_3 = 0x33
local ASCII_4 = 0x34
local ASCII_5 = 0x35
local ASCII_6 = 0x36
local ASCII_7 = 0x37
local ASCII_8 = 0x38
local ASCII_9 = 0x39
local ASCII_COLON = 0x3A
local ASCII_UPPER_A = 0x41
local ASCII_UPPER_E = 0x45
local ASCII_UPPER_F = 0x46
local ASCII_OPENING_SQARE_BRACKET = 0x5B  -- [
local ASCII_BACKSLASH = 0x5C
local ASCII_CLOSING_SQUARE_BRACKET = 0x5D --]
local ASCII_LOWER_A = 0x61
local ASCII_LOWER_B = 0x62
local ASCII_LOWER_E = 0x65
local ASCII_LOWER_F = 0x66
local ASCII_LOWER_N = 0x6E
local ASCII_LOWER_R = 0x72
local ASCII_LOWER_T = 0x74
local ASCII_LOWER_U = 0x75
local ASCII_OPENING_CURLY_BRACE = 0x7B -- {
local ASCII_CLOSING_CURLY_BRACE = 0x7D -- }

local validHexDigits = {
    ["0"] = true,
    ["1"] = true,
    ["2"] = true,
    ["3"] = true,
    ["4"] = true,
    ["5"] = true,
    ["6"] = true,
    ["7"] = true,
    ["8"] = true,
    ["9"] = true,
    ["a"] = true,
    ["b"] = true,
    ["c"] = true,
    ["d"] = true,
    ["e"] = true,
    ["f"] = true,
    ["A"] = true,
    ["B"] = true,
    ["C"] = true,
    ["D"] = true,
    ["E"] = true,
    ["F"] = true,
}

---@diagnostic disable-next-line: undefined-field
local unicode = string.unicode

local validDigits = {
    [ASCII_0] = true,
    [ASCII_1] = true,
    [ASCII_2] = true,
    [ASCII_3] = true,
    [ASCII_4] = true,
    [ASCII_5] = true,
    [ASCII_6] = true,
    [ASCII_7] = true,
    [ASCII_8] = true,
    [ASCII_9] = true,
}

local function readChars(ctx, n)
    local tbl = {}
    for i = 1, n do
        tbl[i] = ctx.currentChar()
        ctx.nextCodepoint()
    end
    return table.concat(tbl)
end

local function parseBoolean(ctx)
    if ctx.currentCodepoint == ASCII_LOWER_T then
        local s = readChars(ctx, 4)
        if s ~= "true" then error("expected true, got " .. s) end
        return true
    elseif ctx.currentCodepoint == ASCII_LOWER_F then
        local s = readChars(ctx, 5)
        if s ~= "false" then error("expected false, got " .. s) end
        return false
    else
        error("expected start of boolean, got " .. ctx.currentChar())
    end
end

local function parseNull(ctx)
    if ctx.currentCodepoint ~= ASCII_LOWER_N then error("expected start of null, got " .. ctx.currentChar()) end
    local s = readChars(ctx, 4)

    if s == "null" then
        return nil
    else
        error("expected null, got " .. s)
    end
end

local function parseNumber(ctx)
    if ctx.currentCodepoint ~= ASCII_MINUS and not validDigits[ctx.currentCodepoint] then
        error("expected start of number, got " .. ctx.currentChar())
    end

    -- todo check for further optimization
    local tbl = {}
    local tblPos = 1
    while validDigits[ctx.currentCodepoint] or ctx.currentCodepoint == ASCII_MINUS or ctx.currentCodepoint == ASCII_PLUS or ctx.currentCodepoint == ASCII_DOT or ctx.currentCodepoint == ASCII_LOWER_E or ctx.currentCodepoint == ASCII_UPPER_E do
        tbl[tblPos] = string.char(ctx.currentCodepoint)
        tblPos = tblPos + 1
        ctx.nextCodepoint()
    end

    local s = table.concat(tbl)
    local n = tonumber(s)

    if n == nil then error("not a number: " .. s) end
    return n
end

local hexCharCodepointToHexValue = {
    [0x30] = 0,  -- '0'
    [0x31] = 1,  -- '1'
    [0x32] = 2,  -- '2'
    [0x33] = 3,  -- '3'
    [0x34] = 4,  -- '4'
    [0x35] = 5,  -- '5'
    [0x36] = 6,  -- '6'
    [0x37] = 7,  -- '7'
    [0x38] = 8,  -- '8'
    [0x39] = 9,  -- '9'
    [0x41] = 10, -- 'A'
    [0x42] = 11, -- 'B'
    [0x43] = 12, -- 'C'
    [0x44] = 13, -- 'D'
    [0x45] = 14, -- 'E'
    [0x46] = 15, -- 'F'
    [0x61] = 10, -- 'a'
    [0x62] = 11, -- 'b'
    [0x63] = 12, -- 'c'
    [0x64] = 13, -- 'd'
    [0x65] = 14, -- 'e'
    [0x66] = 15, -- 'f'
}

local function parseUnicodeSeq(u1, u2, u3, u4)
    local sum = hexCharCodepointToHexValue[u1] * 4096
        + hexCharCodepointToHexValue[u2] * 256
        + hexCharCodepointToHexValue[u3] * 16
        + hexCharCodepointToHexValue[u4]

    return string.char(sum)
end

local function parseString(ctx)
    if ctx.currentCodepoint ~= ASCII_DOUBLE_QUOTE then error("expected start of string, got " .. ctx.currentChar()) end
    ctx.nextCodepoint()
    local done = false
    local escaped = false
    local sb = {}
    local sbPos = 1
    local push = function(s)
        sb[sbPos] = s
        sbPos = sbPos + 1
    end

    while not done do
        local b = ctx.currentCodepoint
        if escaped then
            if b == ASCII_DOUBLE_QUOTE then
                push("\"")
            elseif b == ASCII_BACKSLASH then
                push("\\")
            elseif b == ASCII_FORWARDSLASH then
                push("/")
            elseif b == ASCII_LOWER_N then
                push("\n")
            elseif b == ASCII_LOWER_U then
                local char = parseUnicodeSeq(
                    ctx.nextCodepoint(),
                    ctx.nextCodepoint(),
                    ctx.nextCodepoint(),
                    ctx.nextCodepoint()
                )
                push(char)
            elseif b == ASCII_LOWER_R then
                push("\r")
            elseif b == ASCII_LOWER_T then
                push("\t")
            elseif b == ASCII_LOWER_B then
                push("\b")
            elseif b == ASCII_LOWER_F then
                push("\f")
            else
                error("unsupported escaped symbol " .. ctx.currentChar())
            end
            escaped = false
        elseif b == ASCII_BACKSLASH then
            escaped = true
        elseif b == ASCII_DOUBLE_QUOTE then
            done = true
        elseif b == nil then
            error("json is not terminated properly")
        else
            push(string.char(b))
        end
        ctx.nextCodepoint()
    end

    return table.concat(sb)
end

local function parseValue(ctx)
    local b = ctx.currentCodepoint
    local value

    if b == ASCII_DOUBLE_QUOTE then
        value = parseString(ctx)
    elseif b == ASCII_OPENING_CURLY_BRACE then
        value = parseObject(ctx)
    elseif b == ASCII_OPENING_SQARE_BRACKET then
        value = parseArray(ctx)
    elseif b == ASCII_LOWER_F or b == ASCII_LOWER_T then
        value = parseBoolean(ctx)
    elseif b == ASCII_MINUS or validDigits[b] then
        value = parseNumber(ctx)
    elseif b == ASCII_LOWER_N then
        value = parseNull(ctx)
    else
        error("expected start of a value, got " .. ctx.currentChar())
    end

    ctx.skipWhiteSpace()

    return value
end

parseObject = function(ctx)
    if ctx.currentCodepoint ~= ASCII_OPENING_CURLY_BRACE then error("expected start of object, got " .. ctx.currentChar()) end
    ctx.nextCodepoint()
    ctx.skipWhiteSpace()

    if ctx.currentCodepoint == ASCII_CLOSING_CURLY_BRACE then
        ctx.nextCodepoint()
        ctx.skipWhiteSpace()
        return {}
    end

    local obj = {}

    while true do
        local key = parseString(ctx)
        ctx.skipWhiteSpace()
        if ctx.currentCodepoint ~= ASCII_COLON then error("expected :, got " .. ctx.currentChar()) end
        ctx.nextCodepoint()
        ctx.skipWhiteSpace()
        local value = parseValue(ctx)
        obj[key] = value
        if ctx.currentCodepoint == ASCII_COMMA then
            ctx.nextCodepoint()
            ctx.skipWhiteSpace()
        elseif ctx.currentCodepoint == ASCII_CLOSING_CURLY_BRACE then
            ctx.nextCodepoint()
            ctx.skipWhiteSpace()
            return obj
        else
            error("expected ',' or '}' after object value but got " .. ctx.currentChar())
        end
    end

    return obj
end

parseArray = function(ctx)
    if ctx.currentCodepoint ~= ASCII_OPENING_SQARE_BRACKET then
        error("expected start of array, got " ..
            ctx.currentChar())
    end
    ctx.nextCodepoint()
    ctx.skipWhiteSpace()

    if ctx.currentCodepoint == ASCII_CLOSING_SQUARE_BRACKET then
        ctx.nextCodepoint()
        ctx.skipWhiteSpace()
        return {}
    end

    local tbl = {}
    local tblPos = 1

    while true do
        local value = parseValue(ctx)
        tbl[tblPos] = value
        tblPos = tblPos + 1
        if ctx.currentCodepoint == ASCII_COMMA then
            ctx.nextCodepoint()
            ctx.skipWhiteSpace()
        elseif ctx.currentCodepoint == ASCII_CLOSING_SQUARE_BRACKET then
            ctx.nextCodepoint()
            ctx.skipWhiteSpace()
            return tbl
        else
            error("expected ',' or ']' after array value but got " .. ctx.currentChar())
        end
    end

    return tbl
end

function module.parse(str)
    local ctx = {}
    ctx.pos = 1
    ctx.buffer = str
    ctx.bufferLen = #str
    ctx.currentCodepoint = unicode(str, 1)
    ctx.nextCodepoint = function()
        if ctx.currentCodepoint == nil then error("json is not terminated properly") end
        ctx.pos = ctx.pos + 1
        ctx.currentCodepoint = unicode(ctx.buffer, ctx.pos)
        return ctx.currentCodepoint
    end
    ctx.currentChar = function()
        local b = ctx.currentCodepoint
        if (b == nil) then return "" end
        return string.char(b)
    end
    ctx.skipWhiteSpace = function()
        local b = ctx.currentCodepoint
        while
            b == ASCII_SPACE
            or b == ASCII_HORIZONTAL_TAB
            or b == ASCII_LINE_FEED
            or b == ASCII_CARRIAGE_RETURN
        do
            b = ctx.nextCodepoint()
        end
    end

    ctx.skipWhiteSpace()
    local value = parseValue(ctx)
    ctx.skipWhiteSpace()

    if ctx.currentCodepoint ~= nil then error("json has data past the parsed value") end

    return value
end

return module
