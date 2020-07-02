using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.Common;
using Datatent.Core.Pages;
using Datatent.Core.Scheduler;
using Datatent.Core.Service;

namespace Datatent.Core.Block
{
    /// <summary>
    /// A data block
    /// </summary>
    /// <seealso cref="Datatent.Core.Block.BaseBlock" />
    internal class DataBlock : BaseBlock
    {
        /// <summary>
        /// The header of a data block
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = Constants.BLOCK_HEADER_SIZE)]
        internal struct DataBlockHeader
        {
            /// <summary>
            /// Indicates the start of the block
            /// </summary>
            [FieldOffset(BLOCK_MARKER)]
            public byte Marker;

            /// <summary>
            /// The block id
            /// </summary>
            [FieldOffset(BLOCK_ID)]
            public ushort Id;

            /// <summary>
            /// The block type
            /// </summary>
            [FieldOffset(BLOCK_TYPE)]
            public BlockType Type;

            /// <summary>
            /// The number of pages in the block
            /// </summary>
            [FieldOffset(BLOCK_NUMBER_OF_PAGES)]
            public uint NumberOfPages;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public DataBlockHeader Header { get; set; }

        protected DataPageManager _pageManager;

        /// <summary>
        /// Header position of the number of pages in this block (byte 6-9) of type uint
        /// </summary>
        public const int BLOCK_NUMBER_OF_PAGES = 6;

        public DataBlock(IOResponse response) : base(response)
        {
            _pageManager = new DataPageManager(_memory);
        }

        /// <summary>
        /// Initializes an existing block.
        /// </summary>
        public void InitExisting()
        {
            Header = this.ReadHeader<DataBlockHeader>();
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="typeId">The type identifier.</param>
        /// <returns>True if save is successful and the address of the Document</returns>
        public (bool Saved, Address Address) SaveData(byte[] data, uint typeId)
        {
            var result = _pageManager.SaveContent(data, typeId);

            if (result.Saved)
            {
                Address address = new Address(AddressScope.Document, this.Header.Id, result.PageId, result.DocumentId);
                return (true, address);
            }

            return (false, default);
        }

        /// <summary>
        /// Initializes an empty block.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void InitEmpty(ushort id)
        {
            DataBlockHeader header = new DataBlockHeader();
            header.Marker = 1;
            header.Id = id;
            header.NumberOfPages = 0;
            header.Type = BlockType.Data;

            Header = header;
        }

        /// <summary>
        /// Gets all pages in this block.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<DataPage> GetAllPages()
        {
            var iterator = _memory.Slice(0);
            for (int i = 0; i < Header.NumberOfPages - 1; i++)
            {
                yield return (DataPage) _pageManager.GetPageById((uint) i);
            }
        }
    }
}
