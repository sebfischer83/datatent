using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Datatent.Core.Benchmarks
{
    [HtmlExporter, RPlotExporter, CsvExporter(), CsvMeasurementsExporter(),
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class MemorySpanArrayBenchmark
    {
        [Params(8192, 65536, 1048576, 4194304)]
#pragma warning disable CA1051 // Do not declare visible instance fields
        public int ArraySize;
#pragma warning restore CA1051 // Do not declare visible instance fields

        private byte[] arrayArray;
        private byte[] arraySpan;
        private byte[] arrayMemory;
        private Random rnd;

        private Memory<byte> memory;

        [GlobalSetup]
        public void Setup()
        {
            arrayArray = new byte[ArraySize];
            arraySpan = new byte[ArraySize];
            arrayMemory = new byte[ArraySize];
            rnd = new Random();
        }

        [IterationSetup]
        public void TestSetup()
        {
           
            memory = new Memory<byte>(arrayMemory);
        }

        [IterationCleanup]
        public void TestClean()
        {
            memory = null;
        }

        [Benchmark(Baseline = true)]
        public int ArrayTest()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                arrayArray[i] = (byte)rnd.Next(0, 256);
            }
            for (var i = ArraySize - 1; i > 0; i--)
            {
                arrayArray[i] = (byte)rnd.Next(0, 256);
            }

            return rnd.Next();
        }

        [Benchmark]
        public int SpanTest()
        {
            var span = new Span<byte>(arraySpan);
            for (var i = 0; i < ArraySize; i++)
            {
                span[i] = (byte)rnd.Next(0, 256);
            }
            for (var i = ArraySize - 1; i > 0; i--)
            {
                span[i] = (byte)rnd.Next(0, 256);
            }

            return rnd.Next();
        }

        [Benchmark]
        public int MemoryTest()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                memory.Span[i] = (byte)rnd.Next(0, 256);
            }
            for (var i = ArraySize - 1; i > 0; i--)
            {
                memory.Span[i] = (byte)rnd.Next(0, 256);
            }

            return rnd.Next();
        }
    }
}
