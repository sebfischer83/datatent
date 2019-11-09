using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Service
{
    public class DummyDataProcessingPipeline : IDataProcessingPipeline
    {
        public DataProcessingInformations GetInformations()
        {
            return new DataProcessingInformations(Guid.Empty, Guid.Empty);
        }

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
