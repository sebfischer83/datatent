using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Datatent.Core.IO
{
    internal static class SpanExtensions
    {
        public static byte ReadByte(this Span<byte> span, int offset)
        {
            return span[offset];
        }

        public static unsafe byte[] ReadBytes(this Span<byte> span, int offset, int length)
        {
            var returnArray = new byte[length];

            fixed (byte* bp = span.Slice(offset))
            fixed (byte* rp = returnArray)
            {
                Unsafe.CopyBlock(rp, bp, (uint)length);
            }
            
            return returnArray;
        }

        public static bool ReadBool(this Span<byte> span, int offset)
        {
            return span[offset] != 0;
        }

        public static uint ReadUInt32(this Span<byte> span, int offset)
        {
            return BitConverter.ToUInt32(span.ToArray(), offset);
        }

        public static ushort ReadUInt16(this Span<byte> span, int offset)
        {
            return MemoryMarshal.Read<ushort>(span.Slice(offset));
        }


        public static Guid ReadGuid(this Span<byte> span, int offset)
        {
            var guidSpan = span.Slice(offset, 16);
            return MemoryMarshal.Read<Guid>(guidSpan);
        }

        public static void WriteBytes(this Memory<byte> memory, int offset, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            int i = offset;
            foreach (var b in bytes)
            {
                memory.Span[i] = b;
                i++;
            }
        }

        public static void WriteBytes(this ref Span<byte> span, int offset, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            int i = offset;
            foreach (var b in bytes)
            {
                span[i] = b;
                i++;
            }
        }

        public static void Write<T>(this Span<T> span, int offset, Span<T> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            int i = offset;
            foreach (var b in values)
            {
                span[i] = b;
                i++;
            }
        }

        public static void WriteByte(this Span<byte> span, int offset, byte b)
        {
            span[offset] = b;
        }

        public static void WriteBool(this Span<byte> span, int offset, bool b)
        {
            span[offset] = (byte) (b ? 1 : 0);
        }

        public static void WriteGuid(this Span<byte> span, int offset, Guid id)
        {
            span.WriteBytes(offset, id.ToByteArray());
        }

        public static void WriteUInt32(this Span<byte> span, int offset, uint val)
        {
            var b = BitConverter.GetBytes(val);
            span.WriteBytes(offset, b);
        }
    }
}