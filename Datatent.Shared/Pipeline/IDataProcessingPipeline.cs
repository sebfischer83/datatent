using System;
using System.Runtime.InteropServices;
using Datatent.Shared.Services;

namespace Datatent.Shared.Pipeline
{
    public interface IDataProcessingPipeline
    {
        DataProcessingInformations GetInformations();

        Span<byte> Input(object obj);

        object Output(Span<byte> bytes);
    }

    [StructLayout(LayoutKind.Explicit, Size = Constants.DATABASE_PIPELINE_INFORMATIONS_LENGTH)]
    public readonly struct DataProcessingInformations
    {
        [FieldOffset(0)] public readonly Guid SerializerServiceId;
        [FieldOffset(15)] public readonly Guid EncryptionServiceId;
        [FieldOffset(29)] public readonly Guid CompressionServiceId;

        public DataProcessingInformations(IItemSerializerService serializerService, IEncryptionService encryptionService,
            ICompressionService compressionService)
        {
            this.SerializerServiceId = serializerService.Identifier;
            this.EncryptionServiceId = encryptionService.Identifier;
            this.CompressionServiceId = encryptionService.Identifier;
        }
    }
}
