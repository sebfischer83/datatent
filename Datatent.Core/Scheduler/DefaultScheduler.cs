using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Datatent.Core.Block;
using Datatent.Core.IO;
using Datatent.Core.Memory;
using DynamicData;
using DynamicData.Alias;

namespace Datatent.Core.Scheduler
{
    internal class DefaultScheduler : IDisposable
    {
        private readonly FileSystemServiceBase _fileSystemService;
        private readonly SourceList<IORequest> _fileSystemRequests;
        private readonly SourceList<IORequest> _fileSystemResponses;

        public int WaitingRequests => _fileSystemRequests.Count;
        public int WaitingResponses => _fileSystemResponses.Count;

        public DefaultScheduler(FileSystemServiceBase fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _fileSystemRequests = new SourceList<IORequest>();
            _fileSystemResponses = new SourceList<IORequest>();

            _fileSystemRequests.Connect().Subscribe(async set =>
            {
                if (set.Adds == 0)
                    return;

                foreach (var change in set)
                {
                    if (change.Reason == ListChangeReason.Add)
                    {
                        var item = change.Item.Current;
                        if (item.RequestDirection == IORequestDirection.Read)
                        {
                            var result = await _fileSystemService.Read(item).ConfigureAwait(false);
                            item.Payload = result;
                        }

                        _fileSystemRequests.Remove(item);
                        _fileSystemResponses.Add(item);
                    }
                }
            });
        }

        public Task<IORequest> ScheduleFileSystemRequest(IORequest request)
        {

            var task = Task.Run(async () =>
            {
                var myChangeSet = _fileSystemResponses.Connect();
                var x = myChangeSet.Filter(systemRequest => systemRequest.Id == request.Id);
                _fileSystemRequests.Add(request);

                var set = await x.FirstAsync();

                var el = set.First().Range.First();
                _fileSystemResponses.Remove(el);

                return el;
            });
            
            return task;
        }

        public void Dispose()
        {
            _fileSystemRequests.Dispose();
            _fileSystemResponses.Dispose();
        }
    }

    /// <summary>
    /// A request to the underlying file system
    /// </summary>
    public struct IORequest
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="requestDirection"></param>
        /// <param name="address"></param>
        public IORequest(IORequestDirection requestDirection, Address address)
        {
            Id = Guid.NewGuid();
            RequestDirection = requestDirection;
            Address = address;
            Payload = null;
        }

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
        public ByteMemoryPool.Rental? Payload { get; set; }

        /// <summary>
        /// Creates a new read request
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static IORequest CreateReadRequest(Address address)
        {
            return new IORequest(IORequestDirection.Read, address);
        }

        /// <summary>
        /// Creates a new write request
        /// </summary>
        /// <param name="address"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static IORequest CreateWriteIoRequest(Address address, ByteMemoryPool.Rental payload)
        {
            return new IORequest(IORequestDirection.Write, address) { Payload = payload };
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
}
