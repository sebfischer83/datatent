using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.IO;
using Datatent.Core.Pages;
using Datatent.Core.Service;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using FluentAssertions;
using Xunit;
using Lz4CompressionService = Datatent.Core.Service.Compression.Lz4CompressionService;

namespace Datatent.Core.Tests.Pages
{
    public class PageManagerTest
    {
        private Memory<byte> GenerateTestPages()
        {
            var numberOfPages = 5;
            // generate test pages
            byte[] testArray = new byte[Constants.PAGE_SIZE_INCL_HEADER * numberOfPages];

            Memory<byte> memory = new Memory<byte>(testArray);
            
            // create dummy headers for the 5 pages
            for (uint i = 0; i < numberOfPages; i++)
            {
                var slice = memory.Slice((int) (Constants.PAGE_SIZE_INCL_HEADER * i ));
                slice.WriteBytes(0,UnitTestHelper.GetPageHeader(i, PageType.Data));
            }

            return new Memory<byte>(testArray);
        }

        [Fact]
        public void GetPageByIdSuccessTest()
        {
            ushort id = 3;
            var memoryWithPages = GenerateTestPages();

            DataPageManager pageManager = new DataPageManager(memoryWithPages);

            var page = pageManager.GetPageById(id);

            page.Should().NotBeNull();
            page.Should().BeOfType(typeof(DataPage));
            page.Header.PageType.Should().Be(PageType.Data);
            page.Header.PageId.Should().Be(id);
        }

        [Fact]
        public void GetPageByIdFailIdGreaterThanPossibleTest()
        {
            uint id = Constants.PAGES_PER_DATA_BLOCK + 3;
            var memoryWithPages = GenerateTestPages();

            DataPageManager pageManager = new DataPageManager(memoryWithPages);

            var page = pageManager.GetPageById(id);

            page.Should().NotBeNull();
            page.Should().BeOfType(typeof(NullPage));
            page.Header.PageType.Should().Be(PageType.Null);
            page.Header.PageId.Should().Be(0);
        }

        [Fact]
        public void GetPageByIdFailIdNotExistingTest()
        {
            uint id = Constants.PAGES_PER_DATA_BLOCK - 1;
            var memoryWithPages = GenerateTestPages();

            DataPageManager pageManager = new DataPageManager(memoryWithPages);

            var page = pageManager.GetPageById(id);

            page.Should().NotBeNull();
            page.Should().BeOfType(typeof(NullPage));
            page.Header.PageType.Should().Be(PageType.Null);
            page.Header.PageId.Should().Be(0);
        }
    }
}
