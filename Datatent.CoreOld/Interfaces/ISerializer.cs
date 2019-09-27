using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Interfaces
{
    public interface ISerializer<K>
    {
        byte[] Serialize (K value);

        K Deserialize (byte[] buffer, int offset, int length);

        bool IsFixedSize {
            get;
        }

        int Length {
            get;
        }
    }
}
