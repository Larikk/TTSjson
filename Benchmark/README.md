# Benchmark

Benchmark which compares TTSjson, MoonSharps `json` module and the Json library bundled inside TTS (https://github.com/Berserk-Games/Tabletop-Simulator-Lua-Classes/blob/master/JSON/JSON.lua).

## Setup

1. Execute `fetchAssets.sh` or download the files manually
2. Start the benchmark application with the repository root as working directory

## Output example

```
Warming up...
Running benchmark...
MoonSharpJson took 174ms.
TTSjson took 2011ms.
TTSNativeJSON took 411789ms.
```
