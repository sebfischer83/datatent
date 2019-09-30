using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using Datatent.Core.Algo.Sort;

namespace Datatent.Core.Benchmarks.Algo
{
    [HtmlExporter, RPlotExporter, CsvExporter(), CsvMeasurementsExporter(),
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class SortBenchmark
    {
        [Params(256, 8192, 65536, 4194304, 16000000)]
        //[Params(8192, 4194304)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
        public int ArraySize;

        private const int OpsPerInvoke = 1;
        
        private readonly Consumer consumer = new Consumer();

        private uint[] globalArray;
        private uint[] dummyArray;
        private uint[] array;

        [GlobalSetup]
        public void GlobalSetup()
        {
            globalArray = new uint[ArraySize];
            dummyArray = new uint[ArraySize];

            Random random = new Random();
            for (int i = 0; i < ArraySize; i++)
            {
                globalArray[i] = (uint) random.Next(0, Int32.MaxValue);
                dummyArray[i] = (uint) random.Next(0, Int32.MaxValue);
            }
        }

        [IterationSetup]
        public void Setup()
        {
            array = new uint[ArraySize];
            Buffer.BlockCopy(globalArray, 0, array, 0, globalArray.Length);
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = OpsPerInvoke)]
        public void Linq()
        {
            array.OrderBy(u => u).Consume(consumer);
        }

        //[Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        //public int InsertionSort()
        //{
        //    if (array.Length > 65536)
        //    {
        //        return 0;
        //    }

        //    Datatent.Core.Algo.Sort.Insertion.Sort(ref array);

        //    return array.Length;
        //}

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int Array()
        {
            System.Array.Sort(array);

            return array.Length;
        }

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixLdsPool4Bits()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array);

            return array.Length;
        }

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixLdsNonPool4Bits()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array, false);

            return array.Length;
        }

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixLdsPool8Bits()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array, true, 8);

            return array.Length;
        }

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixLdsNonPool8Bits()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array, false, 8);

            return array.Length;
        }

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixLdsPool2Bits()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array, true, 2);

            return array.Length;
        }

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixLdsNonPool2Bits()
        {
            Datatent.Core.Algo.Sort.RadixLds.Sort(array, false, 2);

            return array.Length;
        }
        
        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixUnsafe()
        {
            array.AsSpan().Sort(dummyArray.AsSpan(), 0, false);

            return array.Length;
        }

        [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
        public int RadixMsd()
        {
            Datatent.Core.Algo.Sort.RadixMsd.Sort(array.AsSpan(), dummyArray.AsSpan(), 0, false);

            return array.Length;
        }
    }
}
