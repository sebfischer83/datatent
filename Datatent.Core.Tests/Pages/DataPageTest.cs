using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Pages;
using Datatent.Core.Service;
using Datatent.Core.IO;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using FluentAssertions;
using Xunit;

namespace Datatent.Core.Tests.Pages
{
    public class DataPageTest
    {
        [Fact]
        public void TryAddContentTest()
        {
            var numberOfPages = 1;
            // generate test pages
            byte[] testArray = new byte[Constants.PAGE_SIZE_INCL_HEADER * numberOfPages];

            Memory<byte> memory = new Memory<byte>(testArray);
            
            // create dummy headers for the 5 pages
            for (uint i = 0; i < numberOfPages; i++)
            {
                var slice = memory.Slice((int) (Constants.PAGE_SIZE_INCL_HEADER * i ));
                slice.WriteBytes(0,UnitTestHelper.GetPageHeader(i, PageType.Data));
            }

            string testContent = "Hello World!!!";
            var bytesContent = Encoding.UTF8.GetBytes(testContent);
            
            var dataPageManager = new DataPageManager(memory);
            var page = (DataPage) dataPageManager.GetPageById(0);

            page.Should().NotBeNull();
            var id = page.TryAddContent(bytesContent, 0);
            id.Should().NotBe(Guid.Empty);

            var document = page.FindById(id);
            document.Should().NotBeNull();
        }

        [Fact]
        public void TryAddManyContentTest()
        {
            var numberOfPages = 1;
            // generate test pages
            byte[] testArray = new byte[Constants.PAGE_SIZE_INCL_HEADER * numberOfPages];

            Memory<byte> memory = new Memory<byte>(testArray);
            
            // create dummy headers for the 5 pages
            for (uint i = 0; i < numberOfPages; i++)
            {
                var slice = memory.Slice((int) (Constants.PAGE_SIZE_INCL_HEADER * i ));
                slice.WriteBytes(0,UnitTestHelper.GetPageHeader(i, PageType.Data));
            }

            string testContent = "Hello World!!!";
            var bytesContent = Encoding.UTF8.GetBytes(testContent);
            
            var dataPageManager = new DataPageManager(memory);
            var page = (DataPage) dataPageManager.GetPageById(0);

            page.Should().NotBeNull();
            var _guids = new Guid[100];
            for (int i = 0; i < 100; i++)
            {
                var res = page.TryAddContent(bytesContent, 0);
                _guids[i] = res;
            }

            _guids.Should().NotBeEmpty();

            var document = page.FindById(_guids[55]);
            document.Should().NotBeNull();
        }
    }
}


