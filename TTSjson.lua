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

local lshift = bit32.lshift
local extractBits = bit32.extract

local function isDigit(b)
    return b ~= nil and b >= ASCII_0 and b <= ASCII_9
end

local function isHexDigit(b)
    return b ~= nil and
    (isDigit(b) or (b >= ASCII_UPPER_A and b <= ASCII_UPPER_F) or (b >= ASCII_LOWER_A and b <= ASCII_LOWER_F))
end

local function readChars(ctx, n)
    local tbl = {}
    for i = 1, n do
        tbl[i] = ctx.currentChar()
        ctx.nextByte()
    end
    return table.concat(tbl)
end

local function parseBoolean(ctx)
    if ctx.currentByte == ASCII_LOWER_T then
        local s = readChars(ctx, 4)
        if s ~= "true" then error("expected true, got " .. s) end
        return true
    elseif ctx.currentByte == ASCII_LOWER_F then
        local s = readChars(ctx, 5)
        if s ~= "false" then error("expected false, got " .. s) end
        return false
    else
        error("expected start of boolean, got " .. ctx.currentChar())
    end
end

local function parseNull(ctx)
    if ctx.currentByte ~= ASCII_LOWER_N then error("expected start of null, got " .. ctx.currentChar()) end
    local s = readChars(ctx, 4)

    if s == "null" then
        return nil
    else
        error("expected null, got " .. s)
    end
end

local function parseNumber(ctx)
    if ctx.currentByte ~= ASCII_MINUS and not isDigit(ctx.currentByte) then
        error("expected start of number, got " .. ctx.currentChar())
    end

    -- todo check for further optimization
    local tbl = {}
    local tblPos = 1
    while isDigit(ctx.currentByte) or ctx.currentByte == ASCII_MINUS or ctx.currentByte == ASCII_PLUS or ctx.currentByte == ASCII_DOT or ctx.currentByte == ASCII_LOWER_E or ctx.currentByte == ASCII_UPPER_E do
        tbl[tblPos] = string.char(ctx.currentByte)
        tblPos = tblPos + 1
        ctx.nextByte()
    end

    local s = table.concat(tbl)
    local n = tonumber(s)

    if n == nil then error("not a number: " .. s) end
    return n
end

local function parseUnicodeSeq(ctx)
    if ctx.currentByte ~= ASCII_LOWER_U then error("expected start of unicode sequence, got " .. ctx.currentChar()) end
    ctx.nextByte()

    -- todo check for more optimization
    local hexDigits = {}
    local isValidHex = true
    for i = 1, 4 do
        local b = ctx.currentByte
        hexDigits[i] = ctx.currentChar()
        if not isHexDigit(b) then
            isValidHex = false
        end
        ctx.nextByte()
    end

    local hex = table.concat(hexDigits)

    if not isValidHex then
        error("invalid unicode sequence: \\u" .. hex)
    end

    return string.char(tonumber(hex, 16))
end

local function parseUtf8(ctx)
    local b1 = ctx.currentByte
    ctx.nextByte()

    local numberOfBytes
    if b1 <= 0x1F or b1 == 0x7F then
        error("unescaped control character encountered: 0x" .. string.format("%02X", b1))
    elseif b1 <= 0x7F then
        return string.char(b1)
    elseif b1 <= 0xDF then
        numberOfBytes = 2
    elseif b1 <= 0xEF then
        numberOfBytes = 3
    elseif b1 <= 0xF7 then
        numberOfBytes = 4
    else
        error("expected valid byte for utf-8 codepoint, got 0x" .. string.format("%02X", b1))
    end

    local tbl = { b1 }
    for i = 2, numberOfBytes do
        tbl[i] = ctx.currentByte
        ctx.nextByte()
    end

    -- Calculate U+wxyz
    local w = 0
    local x = 0
    local y = 0
    local z = 0
    if numberOfBytes == 2 then
        -- Transforms 110xxxyy 10yyzzzz to U+0xyz
        x = extractBits(tbl[1], 2, 3)
        y = lshift(extractBits(tbl[1], 0, 2), 2) + extractBits(tbl[2], 4, 2)
        z = extractBits(tbl[2], 0, 4)
    elseif numberOfBytes == 3 then
        -- Transforms 1110wwww 10xxxxyy 10yyzzzz to U+wxyz
        w = extractBits(tbl[1], 0, 4)
        x = extractBits(tbl[2], 2, 4)
        y = lshift(extractBits(tbl[2], 0, 2), 2) + extractBits(tbl[3], 4, 2)
        z = extractBits(tbl[3], 0, 4)
    elseif numberOfBytes == 4 then
        -- TTS can not display any unicode characters above U+FFFF so we just return � (U+FFFD) in such cases
        return string.char(0xFFFD)
    end

    local sum = lshift(w, 12) + lshift(x, 8) + lshift(y, 4) + z
    return string.char(sum)
end

local function parseString(ctx)
    if ctx.currentByte ~= ASCII_DOUBLE_QUOTE then error("expected start of string, got " .. ctx.currentChar()) end
    ctx.nextByte()
    local done = false
    local escaped = false
    local sb = {}
    local sbPos = 1
    local push = function(s)
        sb[sbPos] = s
        sbPos = sbPos + 1
    end

    while not done do
        local b = ctx.currentByte
        if escaped then
            if b == ASCII_DOUBLE_QUOTE then
                push("\"")
                ctx.nextByte()
            elseif b == ASCII_BACKSLASH then
                push("\\")
                ctx.nextByte()
            elseif b == ASCII_FORWARDSLASH then
                push("/")
                ctx.nextByte()
            elseif b == ASCII_LOWER_N then
                push("\n")
                ctx.nextByte()
            elseif b == ASCII_LOWER_U then
                push(parseUnicodeSeq(ctx))
            elseif b == ASCII_LOWER_R then
                push("\r")
                ctx.nextByte()
            elseif b == ASCII_LOWER_T then
                push("\t")
                ctx.nextByte()
            elseif b == ASCII_LOWER_B then
                push("\b")
                ctx.nextByte()
            elseif b == ASCII_LOWER_F then
                push("\f")
                ctx.nextByte()
            else
                error("unsupported escaped symbol " .. ctx.currentChar())
            end
            escaped = false
        elseif b == ASCII_BACKSLASH then
            escaped = true
            ctx.nextByte()
        elseif b == ASCII_DOUBLE_QUOTE then
            done = true
            ctx.nextByte()
        else
            push(string.char(b))
            ctx.nextByte()
        end
    end

    return table.concat(sb)
end

local function parseValue(ctx)
    local b = ctx.currentByte
    local value

    if b == ASCII_DOUBLE_QUOTE then
        value = parseString(ctx)
    elseif b == ASCII_OPENING_CURLY_BRACE then
        value = parseObject(ctx)
    elseif b == ASCII_OPENING_SQARE_BRACKET then
        value = parseArray(ctx)
    elseif b == ASCII_LOWER_F or b == ASCII_LOWER_T then
        value = parseBoolean(ctx)
    elseif b == ASCII_MINUS or isDigit(b) then
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
    if ctx.currentByte ~= ASCII_OPENING_CURLY_BRACE then error("expected start of object, got " .. ctx.currentChar()) end
    ctx.nextByte()
    ctx.skipWhiteSpace()

    if ctx.currentByte == ASCII_CLOSING_CURLY_BRACE then
        ctx.nextByte()
        ctx.skipWhiteSpace()
        return {}
    end

    local obj = {}

    while true do
        local key = parseString(ctx)
        ctx.skipWhiteSpace()
        if ctx.currentByte ~= ASCII_COLON then error("expected :, got " .. ctx.currentChar()) end
        ctx.nextByte()
        ctx.skipWhiteSpace()
        local value = parseValue(ctx)
        obj[key] = value
        if ctx.currentByte == ASCII_COMMA then
            ctx.nextByte()
            ctx.skipWhiteSpace()
        elseif ctx.currentByte == ASCII_CLOSING_CURLY_BRACE then
            ctx.nextByte()
            ctx.skipWhiteSpace()
            return obj
        else
            error("expected ',' or '}' after object value but got " .. ctx.currentChar())
        end
    end

    return obj
end

parseArray = function(ctx)
    if ctx.currentByte ~= ASCII_OPENING_SQARE_BRACKET then error("expected start of array, got " .. ctx.currentChar()) end
    ctx.nextByte()
    ctx.skipWhiteSpace()

    if ctx.currentByte == ASCII_CLOSING_SQUARE_BRACKET then
        ctx.nextByte()
        ctx.skipWhiteSpace()
        return {}
    end

    local tbl = {}
    local tblPos = 1

    while true do
        local value = parseValue(ctx)
        tbl[tblPos] = value
        tblPos = tblPos + 1
        if ctx.currentByte == ASCII_COMMA then
            ctx.nextByte()
            ctx.skipWhiteSpace()
        elseif ctx.currentByte == ASCII_CLOSING_SQUARE_BRACKET then
            ctx.nextByte()
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
    ctx.currentByte = string.unicode(str, 1)
    ctx.nextByte = function()
        if ctx.currentByte == nil then error("json is not terminated properly") end
        ctx.pos = ctx.pos + 1
        ctx.currentByte = string.unicode(ctx.buffer, ctx.pos)
        return ctx.currentByte
    end
    ctx.currentChar = function()
        local b = ctx.currentByte
        if (b == nil) then return "" end
        return string.char(b)
    end
    ctx.skipWhiteSpace = function()
        local b = ctx.currentByte
        while
            b == ASCII_SPACE
            or b == ASCII_HORIZONTAL_TAB
            or b == ASCII_LINE_FEED
            or b == ASCII_CARRIAGE_RETURN
        do
            b = ctx.nextByte()
        end
    end

    ctx.skipWhiteSpace()
    local value = parseValue(ctx)
    ctx.skipWhiteSpace()

    if ctx.currentByte ~= nil then error("json has data past the parsed value") end

    return value
end

return module
