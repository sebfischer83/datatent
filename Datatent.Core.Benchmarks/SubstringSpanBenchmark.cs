using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Datatent.Core.Benchmarks
{
    [HtmlExporter, RPlotExporter, CsvExporter(), CsvMeasurementsExporter(),
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class SubstringSpanBenchmark
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
        public List<string> _stringsList;

        [GlobalSetup]
        public void Setup()
        {
            Random rand = new Random();
            _stringsList = new List<string>();

            for (int i = 0; i < 92000; i++)
            {
                var str = string.Create(rand.Next(5, 30), rand, (span, random) =>
                {
                    for (int j = 0; j < span.Length; j++)
                    {
                        span[j] = (char) (rand.Next(0, 10) + '0');
                    }
                });
                _stringsList.Add(str);
            }
        }

        [Benchmark(Baseline = true)]
        public int Substring()
        {
            int i = 0;
            char a = 'a';
            foreach (var str in _stringsList)
            {
                var s = str.Substring(3, str.Length - 3);
                if (s.Contains(a, StringComparison.CurrentCultureIgnoreCase))
                    i++;
            }

            return i;
        }

        [Benchmark]
        public int Span()
        {
            int i = 0;
            char a = 'a';
            foreach (var str in _stringsList)
            {
                ReadOnlySpan<char> span = str;
                if (span.Slice(3, str.Length - 3).Contains(a))
                    i++;
            }

            return i;
        }
    }
}
