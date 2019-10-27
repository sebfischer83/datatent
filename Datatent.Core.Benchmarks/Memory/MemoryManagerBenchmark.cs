using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;
using Datatent.Core.Memory;

namespace Datatent.Core.Benchmarks.Memory
{
    [GcForce(false)]
    [HtmlExporter, CsvExporter(), CsvMeasurementsExporter(), MarkdownExporterAttribute.StackOverflow,
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.Method)]
    public class MemoryManagerBenchmark
    {
        [Params(1000, 8000, 64000, 4000000)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
        public int ArraySize;

        [Benchmark(Baseline = true)]
        public int MemoryPoolDefault()
        {
            var x = ArrayPool<byte>.Shared.Rent(ArraySize);
            var l = x.Length;
            ArrayPool<byte>.Shared.Return(x, true);
            return l;
        }

        [Benchmark(Baseline = false)]
        public int MemoryPoolByte()
        {
            using var x = ByteMemoryPool.Shared.Rent(ArraySize);
            var l = x.Memory.Length;
            return l;
        }

        [Benchmark(Baseline = false)]
        public int MemoryManager()
        {
            using var x = new NativeByteMemoryManager(ArraySize);
            var l = x.Memory.Length;
            return l;
        }
    }
}
