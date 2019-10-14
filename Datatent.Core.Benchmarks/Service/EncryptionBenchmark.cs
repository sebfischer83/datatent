using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Benchmarks.Service
{
    [GcForce(false)]
    [HtmlExporter, RPlotExporter, CsvExporter(), CsvMeasurementsExporter(), 
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.Declared)]
    public class EncryptionBenchmark
    {
        [Params(250, 500, 1000, 20000)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
        public int StringSize;

        private byte[] toCompress;
        private string expectedString;
        private AESEncryptionService _aesEncryptionService;
        private TripleDESEncryptionService _tripleDesEncryptionService;

        [GlobalSetup]
        public void Setup()
        {
            var x = RandomData(StringSize);
            toCompress = x.Item1;
            expectedString = x.Item2;
            string pwd = "dsdf4zf48ufr4ju4js";
            _aesEncryptionService = new AESEncryptionService(pwd);
            _tripleDesEncryptionService = new TripleDESEncryptionService(pwd);
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
        public int Aes()
        {
            var res = _aesEncryptionService.Encrypt(toCompress);
            var str = _aesEncryptionService.Decrypt(res.ToArray());
            return str.Length;
        }

        [Benchmark]
        public int TripleDes()
        {
            var res = _tripleDesEncryptionService.Encrypt(toCompress);
            var str = _tripleDesEncryptionService.Decrypt(res.ToArray());
            return str.Length;
        }
    }
}
