using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Service.Serialization
{
    public interface IItemSerializerService
    {
        byte[] Serialize(object item);


    }
}
