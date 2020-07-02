using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Datatent.Core.IO;
using Datatent.Core.Pages;
using Datatent.Core.Service;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using Datatent.Core.Tests;
using Utf8Json.Formatters;

namespace Datatent.Core.Benchmarks.Page
{
    [HtmlExporter, CsvExporter(), CsvMeasurementsExporter(), 
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class FindSyncVsAsync
    {
        private List<int> _indexes = new List<int>();
        private ushort[] _guids;
        
        private DataPage _page;

        private byte[] bytesContent;

        [GlobalSetup]
        public void Setup()
        {
            var numberOfPages = 1;
            // generate test pages
            byte[] testArray = new byte[Constants.PAGE_SIZE_INCL_HEADER * numberOfPages];

            Memory<byte> memory = new Memory<byte>(testArray);
            
            // create dummy headers for the 5 pages
            for (uint i = 0; i < numberOfPages; i++)
            {
                var slice = memory.Slice((int) (Constants.PAGE_SIZE_INCL_HEADER * i ));
                SpanExtensions.WriteBytes(slice, 0,UnitTestHelper.GetPageHeader(i, PageType.Data));
            }

            string testContent = "Hello World!!!";
            bytesContent = Encoding.UTF8.GetBytes(testContent);
            
            var _dataPageManager = new DataPageManager(memory);
            _page = (DataPage) _dataPageManager.GetPageById(0);
            _guids = new ushort[100];
            for (int i = 0; i < 100; i++)
            {
                var res = _page.TryAddContent(bytesContent, 0);
                _guids[i] = res.Id;
            }

            Random random = new Random();
            for (int i = 0; i < Int16.MaxValue * 10; i++)
            {
                _indexes.Add(random.Next(0, 99));
            }
        }

        [Benchmark(Baseline = true)]
        public int Sync()
        {
            foreach (var index in _indexes)
            {
                var result = _page.FindById(_guids[index]);
                return result.Length;
            }

            return 0;
        }

        [Benchmark(Baseline = false)]
        public async Task<int> Async()
        {
            foreach (var index in _indexes)
            {
                var result = await _page.FindByIdAsync(_guids[index]);
                return result.Length;
            }

            return 0;
        }
    }
}
