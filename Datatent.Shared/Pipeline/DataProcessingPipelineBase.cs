using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Shared.Services;

namespace Datatent.Shared.Pipeline
{
    public abstract class DataProcessingPipelineBase : IDataProcessingPipeline
    {
        private readonly IItemSerializerService _serializerService;
        private readonly IEncryptionService _encryptionService;
        private readonly ICompressionService _compressionService;

        protected DataProcessingPipelineBase(IItemSerializerService serializerService, IEncryptionService encryptionService,
            ICompressionService compressionService)
        {
            _serializerService = serializerService;
            _encryptionService = encryptionService;
            _compressionService = compressionService;
        }

        public DataProcessingInformations GetInformations()
        {
            return new DataProcessingInformations(_serializerService, _encryptionService, _compressionService);
        }

        public Span<byte> Input(object obj)
        {
            var bytes = _serializerService.Serialize(obj);
            bytes = _encryptionService.Encrypt(bytes);
            bytes = _compressionService.Compress(bytes);
            return bytes;
        }

        public object Output(Span<byte> bytes)
        {
            var tmp = _compressionService.Decompress(bytes);
            tmp = _encryptionService.Decrypt(tmp);
            return _serializerService.Deserialize(tmp);
        }
    }
}
