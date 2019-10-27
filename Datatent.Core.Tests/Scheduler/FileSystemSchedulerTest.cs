using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datatent.Core.Scheduler;
using FluentAssertions;
using Xunit;

namespace Datatent.Core.Tests.Scheduler
{
    public class FileSystemSchedulerTest
    {
        [Fact]
        public async Task AddAndWaitForResult()
        {
            using FileSystemScheduler fileSystemScheduler = new FileSystemScheduler(null);
            WriteRequest writeRequest = new WriteRequest();
            writeRequest.BlockId = 1;
            var task = fileSystemScheduler.ScheduleFileSystemRequest(writeRequest);

            WriteRequest writeRequest2 = new WriteRequest();
            writeRequest2.BlockId = 1; 
            var x2 = await fileSystemScheduler.ScheduleFileSystemRequest(writeRequest2).ConfigureAwait(false);
            var x1 = await task.ConfigureAwait(false);

            x1.Should().NotBeNull();
            x2.Should().NotBeNull();
            x2.Id.Should().Be(writeRequest2.Id);
            x1.Id.Should().Be(writeRequest.Id);
        }
    }
}
