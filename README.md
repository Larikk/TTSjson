# TTSjson - A json parser for Tabletop Simulator (and Moonsharp)

## Important details about serialization to json

* The following values are serialized: `nil` (except as a value in an object-like table), `boolean`, `number`, `string` and `table`. Every other type (`function`, `userdata` etc.) results in an error.
* Lua tables are serialized as follows:
    * Empty table: Gets serialized into a json array.
    * Array-like table: **All** keys are numerical. Gets serialized into a json array.
    * Object-like table: **All** keys are strings. Gets serialized into a json object.
    * Any table which does not fit into one of the above categories will cause an error.
* Array-like tables:
    * The resulting json array has all the values between the indeces 1 and the highest index of the source table. Gaps are filled with `null`-values. Example: `{ [1] = true, [3] = true }` serializes into `[true, null, true]`.
    * Floating point and negative numbers in array-like tables have undefined behavior. Use at your own risk.
