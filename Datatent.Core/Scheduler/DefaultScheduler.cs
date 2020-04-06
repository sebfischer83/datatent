using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Datatent.Core.Block;
using Datatent.Core.Common;
using Datatent.Core.IO;
using Datatent.Core.Memory;
using DynamicData;
using DynamicData.Alias;

namespace Datatent.Core.Scheduler
{
    internal class DefaultScheduler : IDisposable
    {
        private readonly FileSystemServiceBase _fileSystemService;
        private readonly Channel<ValueTuple<IORequest, TaskCompletionSource<IOResponse>>> _responseChannel;

        public DefaultScheduler(FileSystemServiceBase fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _responseChannel =
                Channel.CreateBounded<ValueTuple<IORequest, TaskCompletionSource<IOResponse>>>(
                    new BoundedChannelOptions(100)
                    {
                        AllowSynchronousContinuations = false,
                        FullMode = BoundedChannelFullMode.Wait,
                        SingleReader = true,
                        SingleWriter = false
                    });
            
            var reader = _responseChannel.Reader;
            Task.Run(Reader);
        }

        private async void Reader()
        {
            var reader = _responseChannel.Reader;
            while (await reader.WaitToReadAsync())
            {
                while (reader.TryRead(out ValueTuple<IORequest, TaskCompletionSource<IOResponse>> tuple))
                {
                    var item = tuple.Item1;
                    IOResponse ioResponse;
                    if (item.RequestDirection == IORequestDirection.Read)
                    {
                        ioResponse = await _fileSystemService.Read(item).ConfigureAwait(false);
                    }
                    else
                    {
                        await _fileSystemService.Write(item).ConfigureAwait(false);
                        ioResponse = new IOResponse(item.Id, item.Address, Array.Empty<byte>());
                    }
                    tuple.Item2.SetResult(ioResponse);
                }
            }
        }


        public async Task<IOResponse> ScheduleFileSystemRequest(IORequest request)
        {
            var writer = _responseChannel.Writer;
            while (await writer.WaitToWriteAsync())
            {
                TaskCompletionSource<IOResponse> source = new TaskCompletionSource<IOResponse>();
                ValueTuple<IORequest, TaskCompletionSource<IOResponse>> tuple = (request, source);
                await writer.WriteAsync(tuple);
                var response = await source.Task.ConfigureAwait(false);
                return response;
            }

            return default;
        }

        public void Dispose()
        {
        }
    }

    public readonly struct IOResponse : IDisposable
    {
        /// <summary>
        /// The request id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The address which should be loaded / written
        /// </summary>
        public Address Address { get; }

        public byte[] Payload { get; }

        public IOResponse(Guid id, Address address, byte[] payload)
        {
            Id = id;
            Address = address;
            Payload = payload;
        }

        public void Dispose()
        {
            if (Payload != null && Payload.Length > 0)
            {
                ArrayPool<byte>.Shared.Return(this.Payload);
            }
        }
    }

    /// <summary>
    /// A request to the underlying file system
    /// </summary>
    public readonly struct IORequest
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="requestDirection"></param>
        /// <param name="address"></param>
        public IORequest(IORequestDirection requestDirection, Address address, IoRequestProperties properties = 0)
        {
            Id = Guid.NewGuid();
            RequestDirection = requestDirection;
            Address = address;
            Payload = Array.Empty<byte>();
            IoRequestProperties = properties;
        }

        public IORequest(IORequestDirection requestDirection, Address address, Memory<byte> payload, IoRequestProperties properties = 0)
        {
            Id = Guid.NewGuid();
            RequestDirection = requestDirection;
            Address = address;
            Payload = payload;
            IoRequestProperties = properties;
        }

        public IORequest(IORequest request, byte[] payload, IoRequestProperties properties = 0)
        {
            Id = request.Id;
            RequestDirection = request.RequestDirection;
            Address = request.Address;
            Payload = payload;
            IoRequestProperties = properties;
        }

        public readonly IoRequestProperties IoRequestProperties { get; }


        /// <summary>
        /// The request id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The address which should be loaded / written
        /// </summary>
        public Address Address { get; }
        
        /// <summary>
        /// Read or write?
        /// </summary>
        public IORequestDirection RequestDirection { get; }

        /// <summary>
        /// The payload
        /// </summary>
        public Memory<byte> Payload { get; }

        /// <summary>
        /// Creates a new read request
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static IORequest CreateReadRequest(Address address, IoRequestProperties properties = 0)
        {
            return new IORequest(IORequestDirection.Read, address, properties);
        }

        /// <summary>
        /// Creates a new write request
        /// </summary>
        /// <param name="address"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static IORequest CreateWriteIoRequest(Address address, Memory<byte> payload,  IoRequestProperties properties = 0)
        {
            return new IORequest(IORequestDirection.Write, address, payload, properties);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Address.FullAddress.GetHashCode();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="request1">The request1.</param>
        /// <param name="request2">The request2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(IORequest request1, IORequest request2)
        {
            if (request1.Address.AreTheSame(request2.Address) && request1.RequestDirection == request2.RequestDirection)
            {
                return true;
            }

            return false;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="request1">The request1.</param>
        /// <param name="request2">The request2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(IORequest request1, IORequest request2)
        {
            return !(request1 == request2);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is IORequest))
            {
                return false;
            }

            var addr = (IORequest) obj;

            return this == addr;
        }
    }

    public enum IORequestDirection
    {
        Read,
        Write
    }

    public enum AddressScope : byte
    {
        Block = 1,
        Page = 2,
        Document = 3
    }

    [Flags]
    public enum IoRequestProperties : short
    {
        None = 0,
        Flush = 1,
        OnlyHeader = 2,
        NoCache = 4
    }
}
