using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Datatent.Core.Memory;
using Datatent.Core.Scheduler;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    internal abstract class FileSystemServiceBase : IDisposable
    {
        private readonly DatatentSettings _settings;
        private readonly ILogger<FileSystemServiceBase> _logger;
        protected Stream _dataFileStream;

        protected FileSystemServiceBase(DatatentSettings settings, ILogger<FileSystemServiceBase> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public void Write(IORequest writeRequest)
        {

        }

        public async Task<ByteMemoryPool.Rental> Read(IORequest writeRequest)
        {
            var pos = writeRequest.Address.ToPosition();
            var buffer = ByteMemoryPool.Shared.Rent((int) pos.length);
            _dataFileStream.Position = pos.offset;
            await _dataFileStream.ReadAsync(buffer.Memory).ConfigureAwait(false);

            return buffer;
        }

        public abstract void Dispose();
    }
}
