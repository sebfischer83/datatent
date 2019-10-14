using Datatent.Core.Benchmarks.IO;

namespace Datatent.Core.Benchmarks
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        static void Main()
        {

            BenchmarkDotNet.Running.BenchmarkRunner.Run<ReadBytesBenchmark>();
        }
    }
}
