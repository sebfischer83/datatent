using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Service.Encryption
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEncryptionService
    {
        Guid Identifier { get; }
        Span<byte> Encrypt(byte[] data);

        Span<byte> Decrypt(byte[] encryptedData);
    }
}
