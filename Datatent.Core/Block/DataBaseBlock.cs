using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.Pages;
using Datatent.Core.Scheduler;
using Datatent.Core.Service;

namespace Datatent.Core.Block
{
    internal class DataBlock : BaseBlock
    {
        [StructLayout(LayoutKind.Explicit, Size = Constants.BLOCK_HEADER_SIZE)]
        internal struct DataBlockHeader
        {
            [FieldOffset(BLOCK_MARKER)]
            public byte Marker;

            [FieldOffset(BLOCK_ID)]
            public ushort Id;

            [FieldOffset(BLOCK_TYPE)]
            public BlockType Type;

            [FieldOffset(BLOCK_NUMBER_OF_PAGES)]
            public uint NumberOfPages;
        }

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

        public void InitExisting()
        {
            Header = this.ReadHeader<DataBlockHeader>();
        }

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

        public void InitEmpty(ushort id)
        {
            DataBlockHeader header = new DataBlockHeader();
            header.Marker = 1;
            header.Id = id;
            header.NumberOfPages = 0;
            header.Type = BlockType.Data;

            Header = header;
        }
        
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
