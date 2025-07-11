local module = {}

local ASCII_HORIZONTAL_TAB = 0x09  -- \t
local ASCII_LINE_FEED = 0x0A       --\n
local ASCII_CARRIAGE_RETURN = 0x0D -- \r
local ASCII_SPACE = 0x20
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
local ASCII_UPPER_B = 0x42
local ASCII_UPPER_C = 0x43
local ASCII_UPPER_D = 0x44
local ASCII_UPPER_E = 0x45
local ASCII_UPPER_F = 0x46
local ASCII_OPENING_SQARE_BRACKET = 0x5B  -- [
local ASCII_BACKSLASH = 0x5C
local ASCII_CLOSING_SQUARE_BRACKET = 0x5D --]
local ASCII_LOWER_A = 0x61
local ASCII_LOWER_B = 0x62
local ASCII_LOWER_C = 0x63
local ASCII_LOWER_D = 0x64
local ASCII_LOWER_E = 0x65
local ASCII_LOWER_F = 0x66
local ASCII_LOWER_L = 0x6C
local ASCII_LOWER_N = 0x6E
local ASCII_LOWER_R = 0x72
local ASCII_LOWER_S = 0x73
local ASCII_LOWER_T = 0x74
local ASCII_LOWER_U = 0x75
local ASCII_OPENING_CURLY_BRACE = 0x7B -- {
local ASCII_CLOSING_CURLY_BRACE = 0x7D -- }

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

local validNumberCharacters = {
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
    [ASCII_MINUS] = true,
    [ASCII_PLUS] = true,
    [ASCII_DOT] = true,
    [ASCII_LOWER_E] = true,
    [ASCII_UPPER_E] = true,
}

-- localize global lookups for performance gains
local tochar = string.char
local substring = string.sub
local concat = table.concat
local tonumber = tonumber
local format = string.format
---@diagnostic disable-next-line: undefined-field
local unicode = string.unicode

local toCharNullsafe
local parseTrue
local parseFalse
local parseNull
local parseNumber
local parseUnicodeSeq
local parseString
local parseValue
local parseObject
local parseArray

-- convencience function for error messages
toCharNullsafe = function(codepoint)
    if codepoint == nil then
        return ""
    else
        return tochar(codepoint)
    end
end

parseTrue = function(ctx)
    if ctx.currentCodepoint ~= ASCII_LOWER_T then error("expected start of true, got " .. toCharNullsafe(ctx.currentCodepoint)) end

    local b2 = ctx.nextCodepoint()
    local b3 = ctx.nextCodepoint()
    local b4 = ctx.nextCodepoint()

    if b2 ~= ASCII_LOWER_R or b3 ~= ASCII_LOWER_U or b4 ~= ASCII_LOWER_E then
        error("expected true, got t" .. toCharNullsafe(b2) .. toCharNullsafe(b3) .. toCharNullsafe(b4))
    end

    ctx.nextCodepoint()
    return true
end

parseFalse = function(ctx)
    if ctx.currentCodepoint ~= ASCII_LOWER_F then error("expected start of false, got " .. toCharNullsafe(ctx.currentCodepoint)) end

    local b2 = ctx.nextCodepoint()
    local b3 = ctx.nextCodepoint()
    local b4 = ctx.nextCodepoint()
    local b5 = ctx.nextCodepoint()

    if b2 ~= ASCII_LOWER_A or b3 ~= ASCII_LOWER_L or b4 ~= ASCII_LOWER_S or b5 ~= ASCII_LOWER_E then
        error("expected false, got f" .. toCharNullsafe(b2) .. toCharNullsafe(b3) .. toCharNullsafe(b4) .. toCharNullsafe(b5))
    end

    ctx.nextCodepoint()
    return false
end

parseNull = function(ctx)
    if ctx.currentCodepoint ~= ASCII_LOWER_N then error("expected start of null, got " .. toCharNullsafe(ctx.currentCodepoint)) end
    local b2 = ctx.nextCodepoint()
    local b3 = ctx.nextCodepoint()
    local b4 = ctx.nextCodepoint()

    if b2 ~= ASCII_LOWER_U or b3 ~= ASCII_LOWER_L or b4 ~= ASCII_LOWER_L then
        error("expected null, got n" .. toCharNullsafe(b2) .. toCharNullsafe(b3) .. toCharNullsafe(b4))
    end

    ctx.nextCodepoint()
    return nil
end

parseNumber = function(ctx)
    if ctx.currentCodepoint ~= ASCII_MINUS and not validDigits[ctx.currentCodepoint] then
        error("expected start of number, got " .. ctx.currentChar())
    end

    local startPos = ctx.pos
    while true do
        if not validNumberCharacters[ctx.currentCodepoint] then
            break
        end
        ctx.nextCodepoint()
    end

    local s = substring(ctx.buffer, startPos, ctx.pos - 1)
    local n = tonumber(s)
    if n == nil then error("not a number: " .. s) end
    return n
end

local hexCharCodepointToHexValue = {
    [ASCII_0] = 0,
    [ASCII_1] = 1,
    [ASCII_2] = 2,
    [ASCII_3] = 3,
    [ASCII_4] = 4,
    [ASCII_5] = 5,
    [ASCII_6] = 6,
    [ASCII_7] = 7,
    [ASCII_8] = 8,
    [ASCII_9] = 9,
    [ASCII_UPPER_A] = 10,
    [ASCII_UPPER_B] = 11,
    [ASCII_UPPER_C] = 12,
    [ASCII_UPPER_D] = 13,
    [ASCII_UPPER_E] = 14,
    [ASCII_UPPER_F] = 15,
    [ASCII_LOWER_A] = 10,
    [ASCII_LOWER_B] = 11,
    [ASCII_LOWER_C] = 12,
    [ASCII_LOWER_D] = 13,
    [ASCII_LOWER_E] = 14,
    [ASCII_LOWER_F] = 15,
}

parseUnicodeSeq = function(u1, u2, u3, u4)
    local sum = hexCharCodepointToHexValue[u1] * 4096
        + hexCharCodepointToHexValue[u2] * 256
        + hexCharCodepointToHexValue[u3] * 16
        + hexCharCodepointToHexValue[u4]

    return tochar(sum)
end

parseString = function(ctx)
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
        elseif b <= 0x1f or b == 0x7F then
            error("unescaped control character encountered: 0x" .. format("%02X", b))
        else
            push(tochar(b))
        end
        ctx.nextCodepoint()
    end

    return concat(sb)
end

parseValue = function(ctx)
    local b = ctx.currentCodepoint
    local value

    if b == ASCII_DOUBLE_QUOTE then
        value = parseString(ctx)
    elseif b == ASCII_OPENING_CURLY_BRACE then
        value = parseObject(ctx)
    elseif b == ASCII_OPENING_SQARE_BRACKET then
        value = parseArray(ctx)
    elseif b == ASCII_LOWER_T then
        value = parseTrue(ctx)
    elseif b == ASCII_LOWER_F then
        value = parseFalse(ctx)
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
        return tochar(b)
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
