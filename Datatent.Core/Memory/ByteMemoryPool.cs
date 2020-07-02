//using System;
//using System.Buffers;
//using System.Collections.Generic;
//using System.Text;

//namespace Datatent.Core.Memory
//{
//    public abstract class ByteMemoryPool : MemoryPool<byte>
//    {
//        private const int POOL_USAGE_BORDER_BYTES = 85000;

//        public override int MaxBufferSize => Int32.MaxValue;

//        public new static ByteMemoryPool.Impl Shared { get; } = new ByteMemoryPool.Impl();

//        public override IMemoryOwner<byte> Rent(int minBufferSize = -1)
//        {
//            return RentCore(minBufferSize);
//        }

//        protected override void Dispose(bool disposing)
//        {
            
//        }

//        private Rental RentCore(int minBufferSize)
//        {
//            return new Rental(minBufferSize);
//        }

//        public sealed class Impl : ByteMemoryPool
//        {
//            public new Rental Rent(int minBufferSize) => RentCore(minBufferSize);
//        }

//        public struct Rental : IMemoryOwner<byte>
//        {
//            private byte[]? _array;
//            private readonly bool _notRented;

//            public Rental(int minBufferSize)
//            {
//                if (minBufferSize < POOL_USAGE_BORDER_BYTES)
//                {
//                    _array = new byte[minBufferSize];
//                    _notRented = true;
//                }
//                else
//                {
//                    _array = ArrayPool<byte>.Shared.Rent(minBufferSize);
//                    _notRented = false;
//                }
//            }

//            public Memory<byte> Memory
//            {
//                get
//                {
//                    if (_array == null)
//                        throw new ObjectDisposedException(nameof(_array));

//                    return new Memory<byte>(_array);
//                }
//            }

//            public void Dispose()
//            {
//                if (_array != null && !_notRented)
//                {
//                    ArrayPool<byte>.Shared.Return(_array, true);
//                    _array = null;
//                }
//                else
//                {
//                    _array = null;
//                }
//            }
//        }
//    }
//}
