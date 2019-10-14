using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Benchmarks.IO
{
    [GcForce(false)]
    [HtmlExporter, RPlotExporter, CsvExporter(), CsvMeasurementsExporter(), 
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.Method)]
    public class ReadBytesBenchmark
    {
        [Params(250, 500, 1000, 20000)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
        public int StringSize;

        private string expectedString;
        private Memory<byte> toRead;
        private byte[] targetBytes;

        [GlobalSetup]
        public void Setup()
        {
            var x = RandomData(StringSize);
            toRead = x.Item1;
            expectedString = x.Item2;
            targetBytes = new byte[toRead.Length];
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public (byte[], string) RandomData(int length)
        {
            Random rand = new Random();
            var str = string.Create(length, rand, (span, random) =>
            {
                for (int j = 0; j < span.Length; j++)
                {
                    span[j] = (char) (rand.Next(0, 10) + '0');
                }
            });
            return (Encoding.UTF8.GetBytes(str), str);
        }
        
        [Benchmark]
        public int BlockCopy()
        {
            System.Buffer.BlockCopy(toRead.Span.ToArray(), 0, targetBytes, 0, targetBytes.Length);
            return targetBytes.Length;
        }

        [Benchmark]
        public unsafe int UnsafeCopyBlock()
        {
            fixed (byte* bp = toRead.Span)
            fixed (byte* rp = targetBytes)
            {
                Unsafe.CopyBlock(rp, bp, (uint)targetBytes.Length);
            }

            return targetBytes.Length;
        }

        [Benchmark]
        public int Slice()
        {
            var slice = toRead.Span.Slice(0, targetBytes.Length);
            var a = slice.ToArray();
            return a.Length;
        }

        [Benchmark(Baseline = true)]
        public int ArrayCopy()
        {
            var slice = toRead.Span.ToArray();
            Array.Copy(slice, targetBytes, targetBytes.Length);
            return targetBytes.Length;
        }
    }
}
