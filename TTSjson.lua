local module = {}

local parseObject
local parseArray

local function lshift(x, by)
    return bit32.lshift(x, by)
end

local function extractBits(x, leastSiginifactStart, width)
    return bit32.extract(x, leastSiginifactStart, width)
end

local function isDigit(c)
    return c == "0" or c == "1" or c == "2" or c == "3" or c == "4" or c == "5" or c == "6" or c == "7" or c == "8" or
        c == "9"
end

local function readChars(ctx, n)
    local tbl = {}
    for i = 1, n do
        tbl[i] = ctx.currentChar
        ctx.advanceChar()
    end
    return table.concat(tbl)
end

local function parseBoolean(ctx)
    if ctx.currentChar == "t" then
        local s = readChars(ctx, 4)
        if s ~= "true" then error("expected true, got " .. s) end
        return true
    elseif ctx.currentChar == "f" then
        local s = readChars(ctx, 5)
        if s ~= "false" then error("expected false, got " .. s) end
        return false
    else
        error("expected start of boolean, got " .. ctx.currentChar)
    end
end

local function parseNull(ctx)
    if ctx.currentChar ~= "n" then error("expected start of null, got " .. ctx.currentChar) end
    local s = readChars(ctx, 4)

    if s == "null" then
        return nil
    else
        error("expected null, got " .. s)
    end
end

local function parseNumber(ctx)
    if ctx.currentChar ~= "-" and not isDigit(ctx.currentChar) then
        error("expected start of number, got " .. ctx.currentChar)
    end

    local tbl = {}
    local tblPos = 1
    while isDigit(ctx.currentChar) or ctx.currentChar == "-" or ctx.currentChar == "+" or ctx.currentChar == "." or ctx.currentChar == "e" or ctx.currentChar == "E" do
        tbl[tblPos] = ctx.currentChar
        tblPos = tblPos + 1
        ctx.advanceChar()
    end

    local s = table.concat(tbl)
    local n = tonumber(s)

    if n == nil then error("not a number: " .. s) end
    return n
end

local function parseUnicodeSeq(ctx)
    if ctx.currentChar ~= "u" then error("expected start of unicode sequence, got " .. ctx.currentChar) end
    ctx.advanceChar()
    local hex = readChars(ctx, 4)
    return string.char(tonumber(hex, 16))
end

local function parseUtf8(ctx)
    local c = ctx.currentChar
    local b1 = string.byte(c, 1)
    ctx.advanceChar()

    local numberOfBytes
    if b1 <= 0x7F then
        return c
    elseif b1 <= 0xDF then
        numberOfBytes = 2
    elseif b1 <= 0xEF then
        numberOfBytes = 3
    elseif b1 <= 0xF7 then
        numberOfBytes = 4
    else
        error("expected valid byte for utf-8 codepoint, got" .. ctx.currentChar)
    end

    local tbl = { b1 }
    for i = 2, numberOfBytes do
        tbl[i] = string.byte(ctx.currentChar, 1)
        ctx.advanceChar()
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
    if ctx.currentChar ~= "\"" then error("expected start of string, got " .. ctx.currentChar) end
    ctx.advanceChar()
    local done = false
    local escaped = false
    local sb = {}
    local sbPos = 1
    local push = function(s)
        sb[sbPos] = s
        sbPos = sbPos + 1
    end

    while not done do
        local c = ctx.currentChar
        if escaped then
            if c == "\"" or c == "\\" or c == "/" then
                push(c)
                ctx.advanceChar()
            elseif c == "n" then
                push("\n")
                ctx.advanceChar()
            elseif c == "u" then
                push(parseUnicodeSeq(ctx))
            elseif c == "r" then
                push("\r")
                ctx.advanceChar()
            elseif c == "t" then
                push("\t")
                ctx.advanceChar()
            elseif c == "b" then
                push("\b")
                ctx.advanceChar()
            elseif c == "f" then
                push("\f")
                ctx.advanceChar()
            else
                error("unsupported escaped symbol" .. c)
            end
            escaped = false
        elseif c == "\\" then
            escaped = true
            ctx.advanceChar()
        elseif c == "\"" then
            done = true
            ctx.advanceChar()
        else
            push(parseUtf8(ctx))
        end
    end

    return table.concat(sb)
end

local function parseValue(ctx)
    local c = ctx.currentChar
    local value

    if c == "\"" then
        value = parseString(ctx)
    elseif c == "{" then
        value = parseObject(ctx)
    elseif c == "[" then
        value = parseArray(ctx)
    elseif c == "f" or c == "t" then
        value = parseBoolean(ctx)
    elseif c == "-" or isDigit(c) then
        value = parseNumber(ctx)
    elseif c == "n" then
        value = parseNull(ctx)
    else
        error("expected start of a value, got " .. c)
    end

    ctx.skipWhiteSpace()

    return value
end

parseObject = function(ctx)
    if ctx.currentChar ~= "{" then error("expected start of object, got " .. ctx.currentChar) end
    ctx.advanceChar()
    ctx.skipWhiteSpace()

    if ctx.currentChar == "}" then
        ctx.advanceChar()
        ctx.skipWhiteSpace()
        return {}
    end

    local obj = {}

    while true do
        local key = parseString(ctx)
        ctx.skipWhiteSpace()
        if ctx.currentChar ~= ":" then error("expected :, got " .. ctx.currentChar) end
        ctx.advanceChar()
        ctx.skipWhiteSpace()
        local value = parseValue(ctx)
        obj[key] = value
        if ctx.currentChar == "," then
            ctx.advanceChar()
            ctx.skipWhiteSpace()
        elseif ctx.currentChar == "}" then
            ctx.advanceChar()
            ctx.skipWhiteSpace()
            return obj
        else
            error("expected ',' or '}' after object value but got " .. ctx.currentChar)
        end
    end

    return obj
end

parseArray = function(ctx)
    if ctx.currentChar ~= "[" then error("expected start of array, got " .. ctx.currentChar) end
    ctx.advanceChar()
    ctx.skipWhiteSpace()

    if ctx.currentChar == "]" then
        ctx.advanceChar()
        ctx.skipWhiteSpace()
        return {}
    end

    local tbl = {}
    local tblPos = 1

    while true do
        local value = parseValue(ctx)
        tbl[tblPos] = value
        tblPos = tblPos + 1
        if ctx.currentChar == "," then
            ctx.advanceChar()
            ctx.skipWhiteSpace()
        elseif ctx.currentChar == "]" then
            ctx.advanceChar()
            ctx.skipWhiteSpace()
            return tbl
        else
            error("expected ',' or ']' after array value but got " .. ctx.currentChar)
        end
    end

    return tbl
end

function module.parse(str)
    local ctx = {}
    ctx.pos = 1
    ctx.buffer = str
    ctx.bufferLen = #str
    ctx.currentChar = string.sub(str, 1, 1)
    ctx.advanceChar = function()
        if ctx.currentChar == "" then error("json is not terminated properly") end
        ctx.pos = ctx.pos + 1
        ctx.currentChar = string.sub(ctx.buffer, ctx.pos, ctx.pos)
        --print("Remaining buffer: >" .. string.sub(ctx.buffer, ctx.pos, ctx.bufferLen))
    end
    ctx.skipWhiteSpace = function()
        local c = ctx.currentChar
        while c == " " or c == "\t" or c == "\n" or c == "\r" do
            ctx.advanceChar()
            c = ctx.currentChar
        end
    end

    ctx.skipWhiteSpace()
    local value = parseValue(ctx)
    ctx.skipWhiteSpace()

    if ctx.currentChar ~= "" then error("json has data past the parsed value") end

    return value
end

return module
