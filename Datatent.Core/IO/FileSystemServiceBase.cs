using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Datatent.Core.Common;
using Datatent.Core.Scheduler;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    /// <summary>
    /// Base class for the file system services
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal abstract class FileSystemServiceBase : IDisposable
    {
        private readonly DatatentSettings _settings;
        private readonly ILogger<FileSystemServiceBase> _logger;
        protected Stream _dataFileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemServiceBase"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        protected FileSystemServiceBase(DatatentSettings settings, ILogger<FileSystemServiceBase> logger)
        {
            _settings = settings;
            _logger = logger;
            _dataFileStream = new MemoryStream();
            _logger.LogInformation($"Init file system service");
        }

        /// <summary>
        /// Writes the data to the underlying stream
        /// </summary>
        /// <param name="writeRequest">The write request.</param>
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

        /// <summary>
        /// Gets the header length for scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Writes the database header.
        /// </summary>
        /// <param name="databaseHeader">The database header.</param>
        public void WriteDatabaseHeader(ref DatabaseHeader databaseHeader)
        {
            Span<byte> headerBytes = stackalloc byte[Constants.DATABASE_HEADER_SIZE];
            _dataFileStream.Position = 0;
            MemoryMarshal.Write(headerBytes, ref databaseHeader);
            _dataFileStream.Write(headerBytes.Slice(0, Constants.DATABASE_HEADER_SIZE));
            _dataFileStream.Flush();
        }

        /// <summary>
        /// Reads the database header.
        /// </summary>
        /// <returns></returns>
        public DatabaseHeader ReadDatabaseHeader()
        {
            Span<byte> headerBytes = stackalloc byte[Constants.DATABASE_HEADER_SIZE];
            _dataFileStream.Position = 0;
            _dataFileStream.Read(headerBytes);
            var header = MemoryMarshal.Read<DatabaseHeader>(headerBytes);
            return header;
        }

        /// <summary>
        /// Reads the data from the underlying stream
        /// </summary>
        /// <param name="readRequest">The read request.</param>
        /// <returns></returns>
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
            await _dataFileStream.ReadAsync(buffer).ConfigureAwait(false);

            return new IOResponse(readRequest.Id, readRequest.Address, buffer);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();
    }
}
