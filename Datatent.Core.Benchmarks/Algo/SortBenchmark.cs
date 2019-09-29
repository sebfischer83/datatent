using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;

namespace Datatent.Core.Benchmarks.Algo
{
    [HtmlExporter, RPlotExporter, CsvExporter(), CsvMeasurementsExporter(),
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class SortBenchmark
    {
        //[Params(8192, 65536, 4194304, 16000000)]
        [Params(8192, 4194304)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
        public int ArraySize;

        private readonly Consumer consumer = new Consumer();

        private uint[] array1;
        private uint[] array2;
        private uint[] array3;

        [IterationSetup]
        public void Setup()
        {
            array1 = new uint[ArraySize];
            array2 = new uint[ArraySize];
            array3 = new uint[ArraySize];

            Random random = new Random();
            for (int i = 0; i < ArraySize; i++)
            {
                array1[i] = (uint) random.Next(0, Int32.MaxValue);
                array2[i] = (uint) random.Next(0, Int32.MaxValue);
                array3[i] = (uint) random.Next(0, Int32.MaxValue);
            }
        }

        [Benchmark(Baseline = true)]
        public void Linq()
        {
            array1.OrderBy(u => u).Consume(consumer);
            array2.OrderBy(u => u).Consume(consumer);
            array3.OrderBy(u => u).Consume(consumer);
        }

        [Benchmark]
        public int Array()
        {
            System.Array.Sort(array1);
            System.Array.Sort(array2);
            System.Array.Sort(array3);

            return array1.Length;
        }

        [Benchmark]
        public int RadixLdsPool()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array1);
            Datatent.Core.Algo.Sort.RadixLds.Sort(array2);
            Datatent.Core.Algo.Sort.RadixLds.Sort(array3);

            return array1.Length;
        }

        [Benchmark]
        public int RadixLdsNonPool()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array1, false);
            Datatent.Core.Algo.Sort.RadixLds.Sort(array2, false);
            Datatent.Core.Algo.Sort.RadixLds.Sort(array3, false);

            return array1.Length;
        }
    }
}
