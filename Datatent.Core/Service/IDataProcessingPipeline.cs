using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Service
{
    public interface IDataProcessingPipeline
    {
        byte[] Input(byte[] bytes);

        byte[] Output(byte[] bytes);
    }
}
