using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Datatent.Core.Scheduler;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    internal sealed class FileReaderFileSystemService : FileSystemService
    {
        private readonly FileStream _dataFileStream;
        
        public FileReaderFileSystemService(DatatentSettings settings, ILogger<FileSystemService> logger) : base(settings, logger)
        {
            _dataFileStream = new FileStream(settings.DataFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.RandomAccess);
        }

        public void Write(WriteRequest writeRequest)
        {

        }

        public override void Dispose()
        {
            _dataFileStream.Dispose();
        }
    }
}
