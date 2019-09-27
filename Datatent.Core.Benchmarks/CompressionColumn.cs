using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Datatent.Core.Benchmarks
{
    public class CompressionColumn : IColumn
    {
        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        {
            //benchmarkCase.
            return string.Empty;
        }

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        {
            return GetValue(summary, benchmarkCase);
        }

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
        {
            return false;
        }

        public bool IsAvailable(Summary summary)
        {
            return true;
        }

        public string Id => nameof(CompressionColumn);
        public string ColumnName { get; set; }
        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Custom;
        public int PriorityInCategory => 0;
        public bool IsNumeric => true;
        public UnitType UnitType => UnitType.Size;
        public string Legend => nameof(CompressionColumn);
    }
}
