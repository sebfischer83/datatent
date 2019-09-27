using System;
using Datatent.Core.Interfaces;

namespace Datatent.Core.Tree
{
	public class TreeStringSerialzier : ISerializer<string>
	{
		public byte[] Serialize (string value)
		{
			return System.Text.Encoding.UTF8.GetBytes (value);
		}

		public string Deserialize (byte[] buffer, int offset, int length)
		{
			return System.Text.Encoding.UTF8.GetString (buffer, offset, length);
		}

		public bool IsFixedSize => false;

        public int Length => throw new InvalidOperationException ();
    }
}

