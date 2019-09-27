using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Datatent.Core.Helper
{
public static class BufferHelper
	{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Guid ReadBufferGuid (byte[] buffer, int bufferOffset)
		{
			var guidBuffer = new byte[16];
			Buffer.BlockCopy (buffer, bufferOffset, guidBuffer, 0, 16);
			return new Guid (guidBuffer);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReadBufferUInt32 (byte[] buffer, int bufferOffset)
		{
			var uintBuffer = new byte[4];
			Buffer.BlockCopy (buffer, bufferOffset, uintBuffer, 0, 4);
			return LittleEndianByteOrderHelper.GetUInt32 (uintBuffer);
		}
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadBufferInt32 (byte[] buffer, int bufferOffset)
		{
			var intBuffer = new byte[4];
			Buffer.BlockCopy (buffer, bufferOffset, intBuffer, 0, 4);
			return LittleEndianByteOrderHelper.GetInt32 (intBuffer);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadBufferInt64 (byte[] buffer, int bufferOffset)
		{
			var longBuffer = new byte[8];
			Buffer.BlockCopy (buffer, bufferOffset, longBuffer, 0, 8);
			return LittleEndianByteOrderHelper.GetInt64 (longBuffer);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ReadBufferDouble (byte[] buffer, int bufferOffset)
		{
			var doubleBuffer = new byte[8];
			Buffer.BlockCopy (buffer, bufferOffset, doubleBuffer, 0, 8);
			return LittleEndianByteOrderHelper.GetDouble (doubleBuffer);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBuffer (double value, byte[] buffer, int bufferOffset)
		{
			Buffer.BlockCopy (LittleEndianByteOrderHelper.GetBytes(value), 0, buffer, bufferOffset, 8);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBuffer (uint value, byte[] buffer, int bufferOffset)
		{
			Buffer.BlockCopy (LittleEndianByteOrderHelper.GetBytes(value), 0, buffer, bufferOffset, 4);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBuffer (long value, byte[] buffer, int bufferOffset)
		{
			Buffer.BlockCopy (LittleEndianByteOrderHelper.GetBytes(value), 0, buffer, bufferOffset, 8);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBuffer (int value, byte[] buffer, int bufferOffset)
		{
			Buffer.BlockCopy (LittleEndianByteOrderHelper.GetBytes((int)value), 0, buffer, bufferOffset, 4);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteBuffer (Guid value, byte[] buffer, int bufferOffset)
		{
			Buffer.BlockCopy (value.ToByteArray(), 0, buffer, bufferOffset, 16);
		}
	}
}
