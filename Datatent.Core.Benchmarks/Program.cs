using System;
using BenchmarkDotNet.Running;
using Datatent.Core.Benchmarks.Algo;

namespace Datatent.Core.Benchmarks
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        static void Main(string[] args)
        {
            BenchmarkDotNet.Running.BenchmarkRunner.Run<SortBenchmark>();
        }
    }
}
