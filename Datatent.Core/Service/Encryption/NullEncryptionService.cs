using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Service.Encryption
{
    public class NullEncryptionService : IEncryptionService
    {
        public Span<byte> Encrypt(byte[] data)
        {
            return data;
        }

        public Span<byte> Decrypt(byte[] encryptedData)
        {
            return encryptedData;
        }
    }
}
