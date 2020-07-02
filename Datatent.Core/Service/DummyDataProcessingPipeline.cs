using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using Datatent.Core.Service.Serialization;
using Datatent.Shared.Pipeline;
using Datatent.Shared.Services;

namespace Datatent.Core.Service
{
    public class DummyDataProcessingPipeline : DataProcessingPipelineBase
    {
        protected DummyDataProcessingPipeline(IItemSerializerService serializerService, IEncryptionService encryptionService, ICompressionService compressionService) : base(serializerService, encryptionService, compressionService)
        {
        }

        public static DummyDataProcessingPipeline Instance => new DummyDataProcessingPipeline(new UTF8JSonSerializer(), new NullEncryptionService(), new UncompressedCompressionService());
    }
}
