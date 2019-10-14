using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.Pages;
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
            public uint Id;

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

        public DataBlock(byte[] buffer, IDataProcessingPipeline processingPipeline) : base(buffer, processingPipeline)
        {
            _pageManager = new DataPageManager(_memory, _processingPipeline);
        }

        public void InitExisting()
        {
            Header = this.ReadHeader<DataBlockHeader>();
        }

        public void SaveData(byte[] data)
        {

        }

        public void InitEmpty(uint id)
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
                yield return FindById((uint) i);
            }
        }

        protected DataPage FindById(uint id)
        {
            if (id > Constants.PAGE_PER_BLOCK)
                throw new ArgumentOutOfRangeException($"A block can't contain a maximum number of {Constants.PAGE_PER_BLOCK} {nameof(id)} {id} is out of range");
            if (id > Header.NumberOfPages)
                throw new ArgumentOutOfRangeException($"This block contains {Header.NumberOfPages} the requested id {id} is larger than the maximum id");

            var offset = Constants.BLOCK_HEADER_SIZE + (id * Constants.PAGE_SIZE);
            DataPage dataPage = new DataPage(_processingPipeline);
            dataPage.InitExisting(_memory.Slice((int) offset, (int) Constants.PAGE_SIZE_INCL_HEADER));

            return dataPage;
        }
    }
}
