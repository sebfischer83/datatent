using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Interfaces
{
    public interface IObjectSerializer
    {
        bool SupportSingleField { get; }

        byte[] Serialize(object model);

        T Deserialize<T>(byte[] bytes);

        V DeserializeSingleField<V>(byte[] bytes, string name);
    }
}
