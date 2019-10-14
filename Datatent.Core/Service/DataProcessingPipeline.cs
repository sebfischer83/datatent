using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class DataProcessingPipeline : IDataProcessingPipeline
    {
        private readonly ICompressionService _compressionService;
        private readonly IEncryptionService _encryptionService;

        public DataProcessingPipeline(ICompressionService compressionService, IEncryptionService encryptionService)
        {
            _compressionService = compressionService;
            _encryptionService = encryptionService;
        }

        public byte[] Input(byte[] bytes)
        {
            var encrypted = _encryptionService.Encrypt(bytes);
            var compressed = _compressionService.Compress(encrypted.ToArray());
            return compressed;
        }

        public byte[] Output(byte[] bytes)
        {
            var uncompressed = _compressionService.Decompress(bytes);
            var decrypted = _encryptionService.Decrypt(uncompressed);

            return decrypted.ToArray();
        }
    }
}
