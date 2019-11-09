using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Datatent.Core.IO;
using Datatent.Core.Service;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Datatent.Core.Tests.Document
{
    public class DocumentTest
    {
        private readonly ITestOutputHelper output;

        public DocumentTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SetAndGetContentTest()
        {
            var array = new byte[Constants.PAGE_SIZE_INCL_HEADER * 2];
            UnitTestHelper.FillArray(ref array, 0x00);
            Memory<byte> documentSlice = new Memory<byte>(array, 500, (int) Constants.PAGE_SIZE);

            Core.Document.Document document = new Core.Document.Document(documentSlice, 0);

            string testString = "This is a test! This is a test! This is a test! This is a test! This is a test! This is a test! This is a test! This is a test!";
            byte[] testContent = Encoding.UTF8.GetBytes(testString);
            document.SetContent(testContent);
            document.Header.ContentLength.Should().Be((uint) testContent.Length);
            var val = document.GetContent();
            val.Should().BeEquivalentTo(testContent);

            output.WriteLine($"Content org is {testContent.Length} and saved {document.Header.ContentLength}");
        }

        [Fact]
        public void WriteDocumentAndLoad()
        {
            var array = new byte[Constants.PAGE_SIZE_INCL_HEADER * 5];
            UnitTestHelper.FillArray(ref array, 0x00);
            Memory<byte> documentSlice = new Memory<byte>(array, 500, (int)Constants.PAGE_SIZE);

            Core.Document.Document document = new Core.Document.Document(documentSlice, 1);

            string testString = "This is a test! This is a test! This is a test! This is a test! This is a test! This is a test! This is a test! This is a test!";
            byte[] testContent = Encoding.UTF8.GetBytes(testString);
            document.Update(testContent);
            document = null;

            Core.Document.Document document2 = new Core.Document.Document(documentSlice);
            var content = document2.GetContent();
            content.Should().BeEquivalentTo(testContent);
        }

        [Fact]
        public void GetNextDocumentSliceAndAdjustOffsetTest()
        {
            var array = new byte[Constants.PAGE_SIZE_INCL_HEADER + 200];
            UnitTestHelper.FillArray(ref array, 0x00);
            string testString = "This is a test! This is a test! This is a test! This is a test! This is a test! This is a test! This is a test! This is a test!";
            byte[] testContent = Encoding.UTF8.GetBytes(testString);

            Memory<byte> documentSlice = new Memory<byte>(array, 0, (int)Constants.PAGE_SIZE);

            ushort id1 = 1;
            ushort id2 = 2;

            Core.Document.Document document = new Core.Document.Document(documentSlice, id1);
            document.Update(testContent);
            document = new Core.Document.Document(documentSlice.Slice((int) (testContent.Length + Constants.DOCUMENT_HEADER_SIZE)), id2);
            document.Update(testContent);

            
            documentSlice = new Memory<byte>(array, 0, (int)Constants.PAGE_SIZE);
            var tuple = Core.Document.Document.GetNextDocumentSliceAndAdjustOffset(ref documentSlice);

            tuple.DocumentId.Should().Be(id1);
            tuple.DocumentSlice.HasValue.Should().BeTrue();
            tuple.DocumentSlice.Value.Length.Should()
                .Be((int) (testContent.Length + Constants.DOCUMENT_HEADER_SIZE));
            tuple = Core.Document.Document.GetNextDocumentSliceAndAdjustOffset(ref documentSlice);
            tuple.DocumentId.Should().Be(id2);
            tuple.DocumentSlice.HasValue.Should().BeTrue();
            tuple.DocumentSlice.Value.Length.Should()
                .Be((int) (testContent.Length + Constants.DOCUMENT_HEADER_SIZE));
            tuple = Core.Document.Document.GetNextDocumentSliceAndAdjustOffset(ref documentSlice);
            tuple.DocumentId.Should().Be(0);
            tuple.DocumentSlice.HasValue.Should().BeFalse();
        }
    }
}
