using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Datatent.Core.Pages
{
    public enum PageType : byte
    {
        Header = 1,
        Data = 2
    }

    public abstract class BasePage
    {
        public int BlockSize { get; }
        /// <summary>
        /// Maximum 4mb (plus 48 bytes for the header) per page, number of blocks 4mb / blocksize.
        /// </summary>
        public int MaxBlocksPerPage => 4194304 / BlockSize;
        
        private readonly Stream _stream;

        public const uint I_PAGE_ID = 0; // bytes 0-3
        public const uint I_PAGE_TYPE = 4; // bytes 4
        public const uint I_PAGE_PREV_ID = 5; // bytes 5-8
        public const uint I_PAGE_NEXT_ID = 9; // bytes 9-12
        public const uint I_PAGE_NUMBER_OF_ITEMS = 13; // bytes 13-16

        
        public const int HEADER_SIZE = 48;

        public abstract PageType PageType { get; }
        public uint PreviousPageId { get; set; }
        public uint NextPageId { get; set; }
        public uint PageId { get; protected set; }
        public uint NumberOfItemsInPage { get; protected set; }

        protected BasePage(Stream stream, int blockSize = 40960)
        {
            BlockSize = blockSize;
            _stream = stream;
        }

        public virtual bool HasEnoughSpaceForData(int lengthOfData)
        {
            int nearestMultiple()
            {
                return ((lengthOfData + BlockSize - 1) / BlockSize) * BlockSize;
            }

            return (MaxBlocksPerPage - NumberOfItemsInPage) > (nearestMultiple());
        }
    }
}
