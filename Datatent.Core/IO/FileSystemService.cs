using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Datatent.Core.Scheduler;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    internal sealed class FileSystemService : FileSystemServiceBase
    {
        public FileSystemService(DatatentSettings settings, ILogger<FileSystemServiceBase> logger) : base(settings, logger)
        {
            _dataFileStream = new FileStream(settings.DataFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.RandomAccess);
        }
        
        public override void Dispose()
        {
            _dataFileStream.Dispose();
        }
    }
}
