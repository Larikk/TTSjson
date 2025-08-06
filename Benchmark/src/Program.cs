
using MoonSharp.Interpreter;

var ttsjson = new TTSjsonParserExecutor();

var executors = new List<IParserExecutor>
{
    // new MoonSharpJsonParserExecutor(),
    ttsjson,
    // new TTSNativeParserExecutor(),
};

string parsingBenchmarkData = File.ReadAllText("./Benchmark/benchmark.json");

void benchmarkParsing()
{
    int iterations = 5;

    Console.WriteLine("Warming up for parsing benchmark...");
    foreach (var executor in executors)
    {
        executor.Parse(parsingBenchmarkData);
    }

    Console.WriteLine("Running parsing benchmark...");
    foreach (var executor in executors)
    {
        var measurements = new List<long>();
        for (int i = 0; i < iterations; i++)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            executor.Parse(parsingBenchmarkData);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            measurements.Add(elapsedMs);
        }
        ;
        var average = measurements.Sum() / measurements.Count;
        Console.WriteLine(executor.GetName() + " took " + average + "ms on average.");
    }
}

void benchmarkWriting()
{
    DynValue writingBenchmarkData = WritingBenchmarkDataBuilder.BuildBenchmarkData(ttsjson.Parse(parsingBenchmarkData));
    int iterations = 5;

    Console.WriteLine("Warming up for writing benchmark...");
    foreach (var executor in executors)
    {
        executor.Write(writingBenchmarkData);
    }

    Console.WriteLine("Running writing benchmark...");
    foreach (var executor in executors)
    {
        var measurements = new List<long>();
        for (int i = 0; i < iterations; i++)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            executor.Write(writingBenchmarkData);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            measurements.Add(elapsedMs);
        }
        ;
        var average = measurements.Sum() / measurements.Count;
        Console.WriteLine(executor.GetName() + " took " + average + "ms on average.");
    }
}

// benchmarkParsing();
benchmarkWriting();

