using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Datatent.Core.Common;
using Datatent.Core.Scheduler;

namespace Datatent.Core.Benchmarks.Flow
{
    [HtmlExporter, CsvExporter(), CsvMeasurementsExporter(),
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
    public class AddressFlowBenchmark
    {
        private int _size = 8096;
        private int _size2 = 1048576;

        private List<Address> _addresses1;
        private List<Address> _addresses2;
        private Random _random;


        [GlobalSetup]
        public void Setup()
        {
            _addresses1 = new List<Address>(_size);
            _addresses2 = new List<Address>(_size);
            _random = new Random();
            Random random = new Random();

            for (int i = 0; i < _size; i++)
            {
                Address address1 = new Address(AddressScope.Document, (ushort)random.Next(1, ushort.MaxValue),
                    (ushort)random.Next(1, ushort.MaxValue), (ushort)random.Next(1, ushort.MaxValue));

                Address address2 = new Address(AddressScope.Document, (ushort)random.Next(1, ushort.MaxValue),
                    (ushort)random.Next(1, ushort.MaxValue), (ushort)random.Next(1, ushort.MaxValue));

                _addresses1.Add(address1);
                _addresses2.Add(address2);
            }
        }

        [Benchmark(Baseline = false, Description = "Calls AreTheSame 8096*8096 times")]
        public long AreTheSameStatic()
        {
            long l = 0;
            for (int i = 0; i < _size; i++)
            {
                Address address1 = _addresses1[i];
                for (int j = 0; j < _size; j++)
                {
                    Address address2 = _addresses2[j];
                    if (Address.AreTheSame(address1, address2))
                        l++;
                }
            }

            return l;
        }

        [Benchmark(Baseline = false, Description = "Calls the Scope and ULong ctor 2^20 times")]
        public ulong ConstructorScopeULong()
        {
            ulong l = 0;
            for (int i = 0; i < _size2; i++)
            {
                Address address = new Address(AddressScope.Document, (ulong)_random.Next(1, int.MaxValue));
                l = address.FullAddress;
            }

            return l;
        }

        [Benchmark(Baseline = false, Description = "Calls ToPosition 8096*8096 times")]
        public long ToPosition()
        {
            long l = 0;
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Address address2 = _addresses2[j];
                    var pos = address2.ToPosition();
                    l = pos.length;
                }
            }

            return l;
        }
    }
}
