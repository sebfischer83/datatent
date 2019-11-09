using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatent.Core.IO;
using Datatent.Core.Scheduler;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Datatent.Core.Tests.Scheduler
{
    public class DefaultSchedulerTest
    {
        [Fact]
        public async Task AddAndWaitForResult()
        {

            using DefaultScheduler fileSystemScheduler = new DefaultScheduler(new MemoryFileSystemService(new DatatentSettings(), Constants.BLOCK_SIZE_INCL_HEADER * 2, new NullLogger<FileSystemServiceBase>()));
            var bytes = new byte[Constants.PAGE_SIZE_INCL_HEADER];
            UnitTestHelper.FillArray(ref bytes, 0x01);

            var address = new Address(AddressScope.Page, 1, 1);
            IORequest writeRequest = new IORequest(IORequestDirection.Write, address, bytes);

            var writeResponse = await fileSystemScheduler.ScheduleFileSystemRequest(writeRequest).ConfigureAwait(false);
            writeResponse.Id.Should().Be(writeRequest.Id);

            IORequest readRequest = IORequest.CreateReadRequest(address);
            var readResponse = await fileSystemScheduler.ScheduleFileSystemRequest(readRequest).ConfigureAwait(false);
            readResponse.Address.FullAddress.Should().Be(address.FullAddress);
            readResponse.Payload.Take(bytes.Length).Should().BeEquivalentTo(bytes);
        }

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

            address = new Address(AddressScope.Page, 1, 1);
            
            address.Block.Should().Be(1);
            address.Page.Should().Be(1);
            address.Document.Should().Be(0);
            address.Scope.Should().Be(AddressScope.Page);
        }
    }
}
