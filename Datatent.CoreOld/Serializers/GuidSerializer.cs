using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Helper;
using Datatent.Core.Interfaces;

namespace Datatent.Core.Serializers
{
    public sealed class GuidSerializer : ISerializer<Guid>
    {
        public byte[] Serialize(Guid value)
        {
            return value.ToByteArray ();
        }

        public Guid Deserialize(byte[] buffer, int offset, int length)
        {
            if (length != 16) {
                throw new ArgumentException (nameof(length));
            }

            return BufferHelper.ReadBufferGuid (buffer, offset);
        }

        public bool IsFixedSize => true;
        public int Length => 16;
    }
}
