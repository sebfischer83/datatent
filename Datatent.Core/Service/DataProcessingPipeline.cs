using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using Datatent.Shared.Pipeline;
using Datatent.Shared.Services;

namespace Datatent.Core.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class DataProcessingPipeline : DataProcessingPipelineBase
    {
        public DataProcessingPipeline(IItemSerializerService serializerService, IEncryptionService encryptionService, ICompressionService compressionService) : base(serializerService, encryptionService, compressionService)
        {
        }
    }
}
