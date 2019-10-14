using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.IO;
using Datatent.Core.Service;
using Datatent.Core.Service.Cache;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Pages
{
    /// <summary>
    /// Smallest unit of data storage.
    /// A specific number of pages are belongs to a block.
    /// A page has a specific size and can contain more than one data object.
    /// </summary>
    /// <remarks>
    /// Every page starts with a header that gives the following informations:
    /// 
    /// </remarks>
    internal abstract class BasePage
    {
        [StructLayout(LayoutKind.Explicit, Size = Constants.PAGE_HEADER_SIZE)]
        internal struct PageHeader
        {
            [FieldOffset(PAGE_ID)]
            public uint PageId;

            [FieldOffset(PAGE_TYPE)]
            public PageType PageType;

            [FieldOffset(PAGE_NEXT_ID)]
            public uint PageNextId;

            [FieldOffset(PAGE_PREV_ID)]
            public uint PagePrevId;

            [FieldOffset(PAGE_NUMBER_OF_ENTRIES)]
            public uint PageNumberOfEntries;

            [FieldOffset(PAGE_NUMBER_OF_FREE_BYTES)]
            public uint PageNumberOfFreeBytes;
        }

        protected readonly IDataProcessingPipeline _processingPipeline;

        public PageHeader Header { get; protected set; }
        public bool IsDirty { get; protected set; }

        /// <summary>
        /// The id of the page, goes from byte 0-3
        /// </summary>
        public const int PAGE_ID = 0;

        /// <summary>
        /// The page type, goes from byte 4-4
        /// </summary>
        /// <see cref="PageType"/>
        public const byte PAGE_TYPE = 4;

        public const int PAGE_PREV_ID = 5;

        public const int PAGE_NEXT_ID = 9;

        public const int PAGE_NUMBER_OF_ENTRIES = 13;

        public const int PAGE_NUMBER_OF_FREE_BYTES = 17;
        
        protected Memory<byte> _pageMemorySlice;
        
        protected BasePage(IDataProcessingPipeline processingPipeline)
        {
            _processingPipeline = processingPipeline;
        }

        public void InitEmpty(Memory<byte> arraySlice, uint pageId, PageType pageType)
        {
            if (arraySlice.Length < Constants.PAGE_SIZE_INCL_HEADER)
                throw new ArgumentException($"{nameof(arraySlice)} is too small to fit page");
            _pageMemorySlice = arraySlice;
            PageHeader header = new PageHeader();
            header.PageId = pageId;
            header.PageType = pageType;
            header.PageNextId = UInt32.MaxValue;
            header.PagePrevId = UInt32.MaxValue;
            header.PageNumberOfEntries = 0;
            header.PageNumberOfFreeBytes = Constants.BLOCK_SIZE;
            Header = header;
        }

        public void InitExisting(Memory<byte> arraySlice)
        {
            if (arraySlice.Length < Constants.PAGE_SIZE_INCL_HEADER)
                throw new ArgumentException( $"{nameof(arraySlice)} is too small to fit page");
            _pageMemorySlice = arraySlice;

            ReadHeader(arraySlice);
        }

        private void ReadHeader(Memory<byte> arraySlice)
        {
            var header = MemoryMarshal.Read<PageHeader>(arraySlice.Span);
            Header = header;
        }

        private void WriteHeader(Memory<byte> arraySlice)
        {
            var pageHeader = Header;
            MemoryMarshal.Write(arraySlice.Span, ref pageHeader);
        }
    }
}
