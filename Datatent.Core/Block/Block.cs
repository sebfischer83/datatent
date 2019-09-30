using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Datatent.Core.IO;

namespace Datatent.Core.Block
{
    internal enum BlockType : byte
    {
        General = 0
    }

    internal abstract class Block
    {
        /// <summary>
        /// Header position of the block id (byte 0-15) of type guid
        /// </summary>
        public const int BLOCK_ID = 0;

        /// <summary>
        /// Header position of the block type (byte 0-15) of type <see cref="BlockType"/>
        /// </summary>
        public const byte BLOCK_TYPE = 4;

        /// <summary>
        /// Header position of the number of pages in this block (byte 5-8) of type uint
        /// </summary>
        public const int BLOCK_NUMBER_OF_PAGES = 5;

        protected Memory<byte> _memory;

        protected List<Document.Document> _documents = new List<Document.Document>();

        public BlockType BlockType { get; private set; }
        public uint NumberOfPages { get; private set; }

        public Guid Id { get; private set; }

        protected Block(Memory<byte> buffer, Guid id, BlockType blockType)
        {
            BlockType = blockType;
            _memory = buffer;
            NumberOfPages = 0;

            this.Id = id;
        }

        protected Block(Memory<byte> buffer)
        {
            _memory = buffer;
            InitFromMemory();
        }

        protected void InitFromMemory()
        {
            this.Id = _memory.Span.ReadGuid(BLOCK_ID);
            this.BlockType = (BlockType) _memory.Span.ReadByte(BLOCK_TYPE);
            this.NumberOfPages = _memory.Span.ReadUInt32(BLOCK_NUMBER_OF_PAGES);
        }

        public static (Memory<byte>? blockSlice, Guid BlockId) GetNextBlockSliceAndAdjustOffset(ref Memory<byte> memory)
        {
            if (memory.IsEmpty || memory.Length < Constants.BLOCK_HEADER_SIZE || memory.Span.ReadByte(0) == 0x00)
                return (null, Guid.Empty);

            var id = memory.Span.ReadGuid(BLOCK_ID);
            var blockSlice = memory.Slice(0, (int) Constants.BLOCK_SIZE_INCL_HEADER);

            memory = memory.Slice((int) Constants.BLOCK_SIZE_INCL_HEADER);

            return (blockSlice, id);
        }
    }
}
