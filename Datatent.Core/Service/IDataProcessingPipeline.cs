using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Service
{
    public interface IDataProcessingPipeline
    {
        DataProcessingInformations GetInformations();

        byte[] Input(byte[] bytes);

        byte[] Output(byte[] bytes);
    }

    [StructLayout(LayoutKind.Explicit, Size = Constants.DATABASE_PIPELINE_INFORMATIONS_LENGTH)]
    public readonly struct DataProcessingInformations
    {
        [FieldOffset(0)] public readonly Guid CompressionServiceId;
        [FieldOffset(15)] public readonly Guid EncryptionServiceId;

        public DataProcessingInformations(Guid compressionServiceId, Guid encryptionServiceId)
        {
            CompressionServiceId = compressionServiceId;
            EncryptionServiceId = encryptionServiceId;
        }
    }
}
