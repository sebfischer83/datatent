using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Datatent.Core.Memory
{
    internal sealed class NativeByteMemoryManager : MemoryManager<byte>
    {
        private IntPtr _memoryPtr;
        private readonly int _length;

        public unsafe NativeByteMemoryManager(int length)
        {
            _length = length;
            _memoryPtr = Marshal.AllocHGlobal(length);
            Unsafe.InitBlock((void*)_memoryPtr, 0, (uint)_length);
        }

        public override Memory<byte> Memory => CreateMemory(_length);

        public override unsafe Span<byte> GetSpan()
        {
            return new Span<byte>(_memoryPtr.ToPointer(), _length);
        }

        public override unsafe MemoryHandle Pin(int elementIndex = 0)
        {
            void* pointer = (void*) ((byte*) _memoryPtr + elementIndex);
            return new MemoryHandle(pointer, default, this);
        }

        public override void Unpin()
        {
            Marshal.FreeHGlobal(_memoryPtr);
            _memoryPtr = IntPtr.Zero;
        }

        protected override void Dispose(bool disposing)
        {
            if (_memoryPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_memoryPtr);
                _memoryPtr = IntPtr.Zero;
            }
        }
    }
}
