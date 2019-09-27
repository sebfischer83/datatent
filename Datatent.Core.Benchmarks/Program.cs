using System;
using BenchmarkDotNet.Running;

namespace Datatent.Core.Benchmarks
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        static void Main(string[] args)
        {
            BenchmarkDotNet.Running.BenchmarkRunner.Run<SubstringSpanBenchmark>();
            BenchmarkDotNet.Running.BenchmarkRunner.Run<CompressionBenchmark>();
            BenchmarkDotNet.Running.BenchmarkRunner.Run<MemorySpanArrayBenchmark>();
        }
    }
}
