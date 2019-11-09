using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Datatent.Core.IO;
using Datatent.Core.Scheduler;
using Datatent.Core.Tests;
using Microsoft.Extensions.Logging.Abstractions;

namespace Datatent.Core.Benchmarks.IO
{
    //[Config(typeof(Config))]
    [HtmlExporter, CsvExporter(), CsvMeasurementsExporter(),
     RankColumn(), KurtosisColumn, SkewnessColumn, StdDevColumn, MeanColumn, MedianColumn, BaselineColumn, MediumRunJob, MemoryDiagnoser, Orderer(SummaryOrderPolicy.Method)]
    public class FileSystemServiceBenchmark
    {

        //private class Config : ManualConfig
        //{
        //    public Config()
        //    {
        //        Add(Job.MediumRun.WithGcServer(true).WithGcForce(false).WithId("GcServerNotForce"));
        //        Add(Job.MediumRun.WithGcServer(true).WithGcForce(true).WithId("GcServerForce"));
        //        Add(Job.MediumRun.WithGcServer(false).WithGcForce(false).WithId("GcWorkstationNotForce"));
        //        Add(Job.MediumRun.WithGcServer(false).WithGcForce(true).WithId("GcWorkstationForce"));
        //        Add(Job.MediumRun.WithGcServer(false).WithGcForce(true).WithGcRetainVm(true).WithId("GcWorkstationForceRetain"));
        //    }
        //}

        [Params(100, 1000, 10000)]
        public int Iterations { get; set; }

        [ParamsSource(nameof(ValuesDataLength))]
        public int DataLength { get; set; }

        public IEnumerable<int> ValuesDataLength => new int[] {512, 2048, 5192, 64000, 128000};
        
        private int[][] accessPattern;

        private byte[] array;

        private Datatent.Core.Scheduler.DefaultScheduler scheduler;

        [GlobalSetup]
        public void Setup()
        {
            array = new byte[DataLength];
            UnitTestHelper.FillArray(ref array, 0x02);
            accessPattern = new int[2][];
            accessPattern[0] = new int[Iterations];
            accessPattern[1] = new int[Iterations];
            Random random = new Random();
            foreach (var i in Enumerable.Range(0, Iterations))
            {
                accessPattern[0][i] = random.Next(1, 5);
                accessPattern[1][i] = random.Next(1, 99);
            }

            scheduler = new Datatent.Core.Scheduler.DefaultScheduler(new MemoryFileSystemService(new DatatentSettings(), Constants.BLOCK_SIZE_INCL_HEADER * 6, new NullLogger<FileSystemServiceBase>()));
        }


        [Benchmark]
        public async Task<Guid> Read()
        {
            Guid guid = Guid.Empty;
            foreach (var i in Enumerable.Range(0, accessPattern[0].Length))
            {
                var b = accessPattern[0][i];
                var p = accessPattern[1][i];
                var request = IORequest.CreateReadRequest(new Address(AddressScope.Page, (ushort) b, (ushort) p));
                var result = await scheduler.ScheduleFileSystemRequest(request).ConfigureAwait(false);
                guid = result.Id;
                ArrayPool<byte>.Shared.Return(result.Payload);
            }

            return guid;
        }
        
        [Benchmark]
        public async Task<Guid> Write()
        {
            Guid guid = Guid.Empty;
            foreach (var i in Enumerable.Range(0, accessPattern[0].Length))
            {
                var b = accessPattern[0][i];
                var p = accessPattern[1][i];
                var request = IORequest.CreateWriteIoRequest(new Address(AddressScope.Page, (ushort) b, (ushort) p), array);
                var result = await scheduler.ScheduleFileSystemRequest(request).ConfigureAwait(false);
                guid = result.Id;
            }

            return guid;
        }
    }
}
