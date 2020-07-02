using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.IO;
using Datatent.Core.Scheduler;
using Datatent.Core.Service;

namespace Datatent.Core.Block
{
    /// <summary>
    /// The block type
    /// </summary>
    internal enum BlockType : byte
    {
        None = 0,
        Data = 1,
        Index = 2,
        Meta = 3
    }

    /// <summary>
    /// Base class for blocks
    /// </summary>
    /// <seealso cref="System.IDisposable" />
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
        
        private readonly IOResponse _ioResponse;

        protected BaseBlock(IOResponse ioResponse)
        {
            _ioResponse = ioResponse;
            _memory = ioResponse.Payload;
        }


        /// <summary>
        /// Reads the header.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadHeader<T>() where T : struct
        {
            return MemoryMarshal.Read<T>(_memory.Span);
        }

        /// <summary>
        /// Writes the header.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header">The header.</param>
        public void WriteHeader<T>(T header) where T : struct
        {
            MemoryMarshal.Write(_memory.Span, ref header);
        }

        /// <summary>
        /// Gets the next block slice and adjust offset.
        /// </summary>
        /// <param name="memory">The memory.</param>
        /// <returns></returns>
        public static (Memory<byte>? blockSlice, ushort BlockId) GetNextBlockSliceAndAdjustOffset(ref Memory<byte> memory)
        {
            if (memory.IsEmpty || memory.Length < Constants.BLOCK_HEADER_SIZE || memory.Span.ReadByte(0) == 0x00)
                return (null, 0);

            var id = memory.Span.ReadUInt16(BLOCK_ID);
            var blockSlice = memory.Slice(0, (int) Constants.DATA_BLOCK_SIZE_INCL_HEADER);

            memory = memory.Slice((int) Constants.DATA_BLOCK_SIZE_INCL_HEADER);

            return (blockSlice, id);
        }
        
        public void Dispose()
        {
           _ioResponse.Dispose();
        }
    }
}
