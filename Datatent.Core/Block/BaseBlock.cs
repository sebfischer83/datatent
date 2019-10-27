using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.IO;
using Datatent.Core.Memory;
using Datatent.Core.Service;

namespace Datatent.Core.Block
{
    internal enum BlockType : byte
    {
        None = 0,
        Data = 1
    }

    internal abstract class BaseBlock : IDisposable
    {
        /// <summary>
        /// Header position of the block marker (byte 0) of type byte, always 1
        /// </summary>
        public const int BLOCK_MARKER = 0;

        /// <summary>
        /// Header position of the block id (byte 1-2) of type ushort
        /// </summary>
        public const int BLOCK_ID = 1;

        /// <summary>
        /// Header position of the block type (byte 3) of type <see cref="BlockType"/>
        /// </summary>
        public const byte BLOCK_TYPE = 3;

        protected Memory<byte> _memory;

        private readonly IMemoryOwner<byte> _buffer;

        protected BaseBlock(IMemoryOwner<byte> memory)
        {
            _buffer = memory;
            _memory = memory.Memory;
        }


        public T ReadHeader<T>() where T : struct
        {
            return MemoryMarshal.Read<T>(_memory.Span);
        }

        public void WriteHeader<T>(T header) where T : struct
        {
            MemoryMarshal.Write(_memory.Span, ref header);
        }
        
        public static (Memory<byte>? blockSlice, ushort BlockId) GetNextBlockSliceAndAdjustOffset(ref Memory<byte> memory)
        {
            if (memory.IsEmpty || memory.Length < Constants.BLOCK_HEADER_SIZE || memory.Span.ReadByte(0) == 0x00)
                return (null, 0);

            var id = memory.Span.ReadUInt16(BLOCK_ID);
            var blockSlice = memory.Slice(0, (int) Constants.BLOCK_SIZE_INCL_HEADER);

            memory = memory.Slice((int) Constants.BLOCK_SIZE_INCL_HEADER);

            return (blockSlice, id);
        }
        
        public void Dispose()
        {
            this._buffer?.Dispose();
        }
    }
}
