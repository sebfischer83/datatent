using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.IO;

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
        
        protected Memory<byte> _arraySlice;

        public uint PageId { get; private set; }

        public PageType PageType { get; protected set; }

        public uint PageNextId { get; private set; }

        public uint PagePrevId { get; private set; }

        public uint PageNumberOfEntries { get; private set; }

        public uint PageNumberOfFreeBytes { get; private set; }
        
        protected BasePage()
        {
        }

        public void InitEmpty(Memory<byte> arraySlice, uint pageId, PageType pageType)
        {
            if (arraySlice.Length < Constants.PAGE_SIZE_INCL_HEADER)
                throw new ArgumentException($"{nameof(arraySlice)} is too small to fit page");
            _arraySlice = arraySlice;
            PageId = pageId;
            PageType = pageType;
            PageNextId = UInt32.MaxValue;
            PagePrevId = UInt32.MaxValue;
            PageNumberOfEntries = 0;
            PageNumberOfFreeBytes = Constants.BLOCK_SIZE;
        }

        public void InitExisting(Memory<byte> arraySlice)
        {
            if (arraySlice.Length < Constants.PAGE_SIZE_INCL_HEADER)
                throw new ArgumentException( $"{nameof(arraySlice)} is too small to fit page");
            _arraySlice = arraySlice;

            ReadHeader(arraySlice);
        }

        private void ReadHeader(Memory<byte> arraySlice)
        {
            this.PageId = arraySlice.Span.ReadUInt32(PAGE_ID);
            this.PageType = (PageType) arraySlice.Span.ReadByte(PAGE_TYPE);
            PageNextId = arraySlice.Span.ReadUInt32(PAGE_NEXT_ID);
            PagePrevId = arraySlice.Span.ReadUInt32(PAGE_PREV_ID);
            PageNumberOfEntries = arraySlice.Span.ReadUInt32(PAGE_NUMBER_OF_ENTRIES);
            PageNumberOfFreeBytes = arraySlice.Span.ReadUInt32(PAGE_NUMBER_OF_FREE_BYTES);
        }
    }
}
