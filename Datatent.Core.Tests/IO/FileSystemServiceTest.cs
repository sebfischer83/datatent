using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Datatent.Core.Common;
using Datatent.Core.IO;
using Datatent.Core.Service;
using Datatent.Shared;
using Datatent.Shared.Pipeline;
using Datatent.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Datatent.Core.Tests.IO
{
    public class FileSystemServiceTest
    {
        [Fact]
        public void WriteAndReadHeader()
        {
            using MemoryFileSystemService service = new MemoryFileSystemService(new DatatentSettings(), Constants.DATABASE_HEADER_SIZE * 5, new NullLogger<FileSystemServiceBase>());

            DatabaseHeader expected = new DatabaseHeader(1, new DataProcessingInformations(Guid.NewGuid(), Guid.NewGuid()));
            service.WriteDatabaseHeader(ref expected);

            DatabaseHeader result = service.ReadDatabaseHeader();

            result.Identifier.Should().Be(result.Identifier);
            result.Version.Should().Be(result.Version);
            result.DataProcessingInformations.CompressionServiceId.Should().NotBeEmpty();
        }
    }
}
