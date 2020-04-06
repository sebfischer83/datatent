using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Datatent.Core.Common;
using Datatent.Core.Scheduler;

namespace Datatent.Core.Benchmarks
{
    [HtmlExporter, CsvExporter(), CsvMeasurementsExporter(),
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class AddressBenchmark
    {
        private List<Address> _addresses1;
        private List<Address> _addresses2;

        [Params(1024, 8192)]
        public int ArraySize;

        [GlobalSetup]
        public void Setup()
        {
            _addresses1 = new List<Address>(ArraySize);
            _addresses2 = new List<Address>(ArraySize);

            Random random = new Random();

            for (int i = 0; i < ArraySize; i++)
            {
                Address address1 = new Address(AddressScope.Page, (ushort) random.Next(1, ushort.MaxValue),
                    (ushort) random.Next(1, ushort.MaxValue), (ushort) random.Next(1, ushort.MaxValue));

                Address address2 = new Address(AddressScope.Page, (ushort) random.Next(1, ushort.MaxValue),
                    (ushort) random.Next(1, ushort.MaxValue), (ushort) random.Next(1, ushort.MaxValue));

                _addresses1.Add(address1);
                _addresses2.Add(address2);
            }
        }

        [Benchmark(Baseline = true)]
        public long ObjEquals()
        {
            long l = 0;
            for (int i = 0; i < ArraySize; i++)
            {
                Address address1 = _addresses1[i];
                for (int j = 0; j < ArraySize; j++)
                {
                    Address address2 = _addresses2[j];
                    if (address1.Equals(address2))
                        l++;
                }
            }

            return l;
        }

        [Benchmark(Baseline = false)]
        public long AreTheSameStatic()
        {
            long l = 0;
            for (int i = 0; i < ArraySize; i++)
            {
                Address address1 = _addresses1[i];
                for (int j = 0; j < ArraySize; j++)
                {
                    Address address2 = _addresses2[j];
                    if (Address.AreTheSame(address1, address2))
                        l++;
                }
            }

            return l;
        }

        [Benchmark(Baseline = false)]
        public long AreTheSameInstance()
        {
            long l = 0;
            for (int i = 0; i < ArraySize; i++)
            {
                Address address1 = _addresses1[i];
                for (int j = 0; j < ArraySize; j++)
                {
                    Address address2 = _addresses2[j];
                    if (address1.AreTheSame(address2))
                        l++;
                }
            }

            return l;
        }

        [Benchmark(Baseline = false)]
        public long EqualsOperator()
        {
            long l = 0;
            for (int i = 0; i < ArraySize; i++)
            {
                Address address1 = _addresses1[i];
                for (int j = 0; j < ArraySize; j++)
                {
                    Address address2 = _addresses2[j];
                    if (address1 == address2)
                        l++;
                }
            }

            return l;
        }
    }
}
