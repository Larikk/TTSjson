curl --output benchmark.json "https://db.ygoprodeck.com/api/v7/cardinfo.php?attribute=wind&enddate=2018-01-01"
curl --output TTSNativeJSON.ttslua "https://raw.githubusercontent.com/Berserk-Games/Tabletop-Simulator-Lua-Classes/refs/heads/master/JSON/JSON.lua"
sed -i 's/""/"/g' TTSNativeJSON.ttslua # The file on github has doubled quotes for some reason
