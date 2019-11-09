using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Datatent.Core.Service.Encryption
{
    public class NullEncryptionService : IEncryptionService
    {
        private static readonly Guid IDENTIFIER = Guid.Parse("AD1C84B9-9187-499A-9F2F-7FEBD39F9548");
        
        public Guid Identifier => IDENTIFIER;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> Encrypt(byte[] data)
        {
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> Decrypt(byte[] encryptedData)
        {
            return encryptedData;
        }
    }
}
