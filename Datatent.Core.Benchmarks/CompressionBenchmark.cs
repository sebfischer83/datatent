using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Order;
using K4os.Compression.LZ4;

namespace Datatent.Core.Benchmarks
{
    [HtmlExporter, RPlotExporter, CsvExporter(), CsvMeasurementsExporter(), 
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class CompressionBenchmark
    {
        [Params(8192, 65536, 1048576, 4194304)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
        public int ArraySize;
        
        private byte[] toCompress;

        [GlobalSetup]
        public void Setup()
        {
            toCompress = RandomData(ArraySize);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public byte[] RandomData(int length)
        {
            var data = new byte[length];
            var rnd = new Random ();
            for (var i = 0; i < data.Length; i++) {
                data[i] = (byte)rnd.Next (0, 256);
            }
            return data;
        }

        [Benchmark(Baseline = true)]
        public int CompressWithLz4Fast()
        {
            var target = new byte[LZ4Codec.MaximumOutputSize(toCompress.Length)];
            var result = LZ4Codec.Encode(toCompress, 0, toCompress.Length, target, 0, target.Length, LZ4Level.L00_FAST);
            return result;
        }

        [Benchmark]
        public int CompressWithLz405HC()
        {
            var target = new byte[LZ4Codec.MaximumOutputSize(toCompress.Length)];
            var result = LZ4Codec.Encode(toCompress, 0, toCompress.Length, target, 0, target.Length, LZ4Level.L05_HC);
            return result;
        }

        [Benchmark]
        public int CompressWithLz4L12Max()
        {
            var target = new byte[LZ4Codec.MaximumOutputSize(toCompress.Length)];
            var result = LZ4Codec.Encode(toCompress, 0, toCompress.Length, target, 0, target.Length, LZ4Level.L12_MAX);
            return result;
        }

        [Benchmark]
        public int CompressWithBZip2Level1()
        {
            using (var input = new MemoryStream(toCompress))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(input, memoryStream, false, 1);
                return (int) memoryStream.Length;
            }
        }

        [Benchmark]
        public int CompressWithBZip2Level9()
        {
            using (var input = new MemoryStream(toCompress))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(input, memoryStream, false, 9);
                return (int) memoryStream.Length;
            }
        }
    }
}
