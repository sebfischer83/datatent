using System;
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
using DynamicData;
using DynamicData.Alias;

namespace Datatent.Core.Scheduler
{
    internal class FileSystemScheduler : IDisposable
    {
        private readonly FileSystemService _fileSystemService;
        private readonly SourceList<IFileSystemRequest> _fileSystemRequests;
        private readonly SourceList<IFileSystemRequest> _fileSystemResponses;

        public int WaitingRequests => _fileSystemRequests.Count;
        public int WaitingResponses => _fileSystemResponses.Count;

        public FileSystemScheduler(FileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _fileSystemRequests = new SourceList<IFileSystemRequest>();
            _fileSystemResponses = new SourceList<IFileSystemRequest>();

            _fileSystemRequests.Connect().Subscribe(set =>
            {
                if (set.Adds == 0)
                    return;

                foreach (var change in set)
                {
                    if (change.Reason == ListChangeReason.Add)
                    {
                        //change.Item.Current.Payload = new DataBlock(new byte[10234]);
                        _fileSystemRequests.Remove(change.Item.Current);
                        _fileSystemResponses.Add(change.Item.Current);
                    }
                }
            });
        }

        public Task<IFileSystemRequest> ScheduleFileSystemRequest(IFileSystemRequest request)
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

    internal interface IFileSystemRequest
    {
        Guid Id { get; }

        ushort BlockId { get; }

        Block.BaseBlock Payload { get; set; }
    }

    internal class WriteRequest : IFileSystemRequest
    {
        public Guid Id { get; set; }
        public ushort BlockId { get; set; }
        public Block.BaseBlock? Payload { get; set; }

        public WriteRequest()
        {
            Id = Guid.NewGuid();
        }
    }

    internal class ReadRequest : IFileSystemRequest
    {
        public Guid Id { get; set; }
        public ushort BlockId { get; set; }
        public Block.BaseBlock? Payload { get; set; }

        public ReadRequest()
        {
            Id = Guid.NewGuid();
        }
    }
}
