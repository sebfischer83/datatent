using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Service
{
    public class DummyDataProcessingPipeline : IDataProcessingPipeline
    {
        public byte[] Input(byte[] bytes)
        {
            return bytes;
        }

        public byte[] Output(byte[] bytes)
        {
            return bytes;
        }
    }
}
