using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    internal class MemoryFileSystemService : FileSystemServiceBase
    {
        public MemoryFileSystemService(DatatentSettings settings, long size, ILogger<FileSystemServiceBase> logger) : base(settings, logger)
        {
            _dataFileStream = new MemoryStream(new byte[size], true);
        }

        public override void Dispose()
        {
            
        }
    }
}
