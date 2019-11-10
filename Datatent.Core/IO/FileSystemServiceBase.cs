using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Datatent.Core.Common;
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

        public async Task Write(IORequest writeRequest)
        {
            var pos = writeRequest.Address.ToPosition();
            _dataFileStream.Position = pos.offset;
            var length = pos.length;
            if ((writeRequest.IoRequestProperties & IoRequestProperties.OnlyHeader) == IoRequestProperties.OnlyHeader)
            {
                length = (uint) GetHeaderLengthForScope(writeRequest.Address.Scope);
            }

            await _dataFileStream.WriteAsync(writeRequest.Payload.Slice(0, (int) length)).ConfigureAwait(false);

            if ((writeRequest.IoRequestProperties & IoRequestProperties.Flush) == IoRequestProperties.Flush)
            {
                await _dataFileStream.FlushAsync().ConfigureAwait(false);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetHeaderLengthForScope(AddressScope  scope)
        {
            return scope switch
            {
                AddressScope.Block => Constants.BLOCK_HEADER_SIZE,
                AddressScope.Page => Constants.PAGE_HEADER_SIZE,
                AddressScope.Document => Constants.DOCUMENT_HEADER_SIZE,
                _ => throw new ArgumentOutOfRangeException(nameof(scope))
            };
        }

        public void WriteDatabaseHeader(ref DatabaseHeader databaseHeader)
        {
            Span<byte> headerBytes = stackalloc byte[Constants.DATABASE_HEADER_SIZE];
            _dataFileStream.Position = 0;
            MemoryMarshal.Write(headerBytes, ref databaseHeader);
            _dataFileStream.Write(headerBytes.Slice(0, Constants.DATABASE_HEADER_SIZE));
            _dataFileStream.Flush();
        }

        public DatabaseHeader ReadDatabaseHeader()
        {
            Span<byte> headerBytes = stackalloc byte[Constants.DATABASE_HEADER_SIZE];
            _dataFileStream.Position = 0;
            _dataFileStream.Read(headerBytes);
            var header = MemoryMarshal.Read<DatabaseHeader>(headerBytes);
            return header;
        }
        
        public async Task<IOResponse> Read(IORequest readRequest)
        {
            var pos = readRequest.Address.ToPosition();
            var length = pos.length;

            if ((readRequest.IoRequestProperties & IoRequestProperties.OnlyHeader) == IoRequestProperties.OnlyHeader)
            {
                length = (uint) GetHeaderLengthForScope(readRequest.Address.Scope);
            }

            byte[] buffer = ArrayPool<byte>.Shared.Rent((int) length);
            _dataFileStream.Position = pos.offset;
            Memory<byte> b = buffer;
            await _dataFileStream.ReadAsync(buffer).ConfigureAwait(false);

            return new IOResponse(readRequest.Id, readRequest.Address, buffer);
        }

        public abstract void Dispose();
    }
}
