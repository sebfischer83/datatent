using System;
using Datatent.Core.Helper;
using Datatent.Core.Interfaces;

namespace Datatent.Core.Tree
{
	public class TreeLongSerializer : ISerializer<long>
	{
		public byte[] Serialize (long value)
		{
			return LittleEndianByteOrderHelper.GetBytes (value);
		}

		public long Deserialize (byte[] buffer, int offset, int length)
		{
			if (length != 8) {
				throw new ArgumentException ("Invalid length: " + length);
			}
			
			return BufferHelper.ReadBufferInt64 (buffer, offset);
		}

		public bool IsFixedSize {
			get {
				return true;
			}
		}

		public int Length {
			get {
				return 8;
			}
		}
	}
}

