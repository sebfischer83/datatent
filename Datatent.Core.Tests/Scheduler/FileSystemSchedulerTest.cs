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
        //[Fact]
        //public async Task AddAndWaitForResult()
        //{

        //    //using DefaultScheduler fileSystemScheduler = new DefaultScheduler(null);
        //    //WriteRequest writeRequest = new WriteRequest();
        //    //writeRequest.BlockId = 1;
        //    //var task = fileSystemScheduler.ScheduleFileSystemRequest(writeRequest);

        //    //WriteRequest writeRequest2 = new WriteRequest();
        //    //writeRequest2.BlockId = 1; 
        //    //var x2 = await fileSystemScheduler.ScheduleFileSystemRequest(writeRequest2).ConfigureAwait(false);
        //    //var x1 = await task.ConfigureAwait(false);

        //    //x1.Should().NotBeNull();
        //    //x2.Should().NotBeNull();
        //    //x2.Id.Should().Be(writeRequest2.Id);
        //    //x1.Id.Should().Be(writeRequest.Id);
        //}

        [Fact]
        public void TestAddressCreation()
        {
            Address address = new Address(AddressScope.Document, 20, 5, 11);

            address.Block.Should().Be(20);
            address.Page.Should().Be(5);
            address.Document.Should().Be(11);
            address.Scope.Should().Be(AddressScope.Document);


            address = new Address(AddressScope.Block, ushort.MaxValue, ushort.MaxValue, 0);

            address.Block.Should().Be(ushort.MaxValue);
            address.Page.Should().Be(ushort.MaxValue);
            address.Document.Should().Be(0);
            address.Scope.Should().Be(AddressScope.Block);

            var l = address.FullAddress;
            address = new Address(AddressScope.Page, l);
            
            address.Block.Should().Be(ushort.MaxValue);
            address.Page.Should().Be(ushort.MaxValue);
            address.Document.Should().Be(0);
            address.Scope.Should().Be(AddressScope.Page);

            address = new Address(AddressScope.Block, l);
            
            address.Block.Should().Be(ushort.MaxValue);
            address.Page.Should().Be(ushort.MaxValue);
            address.Document.Should().Be(0);
            address.Scope.Should().Be(AddressScope.Block);
        }
    }
}
