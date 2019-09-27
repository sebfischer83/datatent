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

    internal class Block : IDisposable
    {
        /// <summary>
        /// Block Id, bytes 0-15
        /// </summary>
        public const uint BLOCK_ID = 0;

        /// <summary>
        /// Block type, byte 16-16
        /// </summary>
        public const byte BLOCK_TYPE = 4;

        /// <summary>
        /// Number of pages in this block, bytes 5-8
        /// </summary>
        public const uint BLOCK_NUMBER_OF_PAGES = 5;

        protected Memory<byte> _memory;
        protected byte[] _buffer;

        public Guid Id { get; private set; }

        public Block(byte[] buffer, Guid id)
        {
            _buffer = buffer;
            _memory = new Memory<byte>(buffer);
            this.Id = id;
        }

        public static Block CreateNew(byte[] buffer, Guid id)
        {
            var block = new Block(buffer, id);

            return block;
        }

        private void ReleaseUnmanagedResources()
        {
            ArrayPool<byte>.Shared.Return(_buffer, true);
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Block()
        {
            Dispose(false);
        }
    }
}
