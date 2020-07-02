using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


[assembly: InternalsVisibleTo("Datatent.Core.Tests")]
[assembly: InternalsVisibleTo("Datatent.Core.Benchmarks")]
[assembly: InternalsVisibleTo("Datatent.DummyApplication")]
namespace Datatent.Core
{
    public class DatatentSettings
    {
        public string? DataFile { get; set; }
    }
}
