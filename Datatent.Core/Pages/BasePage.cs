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
    /// <see cref="BasePage.PageHeader"/>
    /// </remarks>
    internal abstract class BasePage
    {
        /// <summary>
        /// The header of every page, the header start until the PageTypeId is the same for every type
        /// but the following can be different.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = Constants.PAGE_HEADER_SIZE)]
        internal struct PageHeader
        {
            /// <summary>
            /// The page identifier
            /// </summary>
            [FieldOffset(PAGE_ID)]
            public ushort PageId;

            [FieldOffset(PAGE_TYPE)]
            public PageType PageType;

            [FieldOffset(PAGE_NEXT_ID)]
            public ushort PageNextId;

            [FieldOffset(PAGE_PREV_ID)]
            public ushort PagePrevId;

            [FieldOffset(PAGE_NUMBER_OF_ENTRIES)]
            public ushort PageNumberOfEntries;

            [FieldOffset(PAGE_NEXT_DOCUMENT_ID)]
            public ushort PageNextDocumentId;

            [FieldOffset(PAGE_NUMBER_OF_FREE_BYTES)]
            public uint PageNumberOfFreeBytes;

            [FieldOffset(PAGE_TYPE_ID)]
            public uint PageTypeId;
        }

        public PageHeader Header;
        public bool IsDirty { get; protected set; }

        /// <summary>
        /// The id of the page, goes from byte 0-1
        /// </summary>
        public const int PAGE_ID = 0;

        /// <summary>
        /// The page type, goes from byte 2-2
        /// </summary>
        /// <see cref="PageType"/>
        public const byte PAGE_TYPE = 2;

        public const int PAGE_PREV_ID = 3;

        public const int PAGE_NEXT_ID = 5;

        public const int PAGE_NUMBER_OF_ENTRIES = 7;

        public const int PAGE_NEXT_DOCUMENT_ID = 9;

        public const int PAGE_NUMBER_OF_FREE_BYTES = 11;

        public const int PAGE_TYPE_ID = 14;
        
        protected Memory<byte> _pageMemorySlice;
        
        protected BasePage()
        {
        }

        public void InitEmpty(Memory<byte> arraySlice, ushort pageId, PageType pageType)
        {
            if (arraySlice.Length < Constants.PAGE_SIZE_INCL_HEADER)
                throw new ArgumentException($"{nameof(arraySlice)} is too small to fit page");
            _pageMemorySlice = arraySlice;
            PageHeader header = new PageHeader
            {
                PageId = pageId,
                PageType = pageType,
                PageNextId = ushort.MaxValue,
                PagePrevId = ushort.MaxValue,
                PageNumberOfEntries = 0,
                PageNextDocumentId = 1,
                PageNumberOfFreeBytes = Constants.DATA_BLOCK_SIZE
            };
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
