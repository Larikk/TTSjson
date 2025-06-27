
var executors = new List<IParserExecutor>
{
    // new MoonSharpJsonParserExecutor(),
    new TTSjsonParserExecutor(),
    // new TTSNativeParserExecutor(),
};

string benchmarkData = File.ReadAllText("./Benchmark/benchmark.json");
int iterations = 5;

Console.WriteLine("Warming up...");
foreach (var executor in executors)
{
    executor.Parse(benchmarkData);
}

Console.WriteLine("Running benchmark...");
foreach (var executor in executors)
{
    var measurements = new List<long>();
    for (int i = 0; i < iterations; i++)
    {

        var watch = System.Diagnostics.Stopwatch.StartNew();
        executor.Parse(benchmarkData);
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        measurements.Add(elapsedMs);
    };
    var average = measurements.Sum() / measurements.Count;
    Console.WriteLine(executor.GetName() + " took " + average + "ms on average.");
}
