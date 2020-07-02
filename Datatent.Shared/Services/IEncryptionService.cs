using System;

namespace Datatent.Shared.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEncryptionService : IPipelineService
    {
        Span<byte> Encrypt(Span<byte> data);

        Span<byte> Decrypt(Span<byte> encryptedData);
    }
}
