using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Datatent.Core.Service.Encryption
{
    public class TripleDESEncryptionService : IEncryptionService
    {
        private readonly string _password;
        private readonly int _iterations = 25;
        private readonly int _keySize = 192;
        private readonly SHA256CryptoServiceProvider _sha1 = new SHA256CryptoServiceProvider();
        private readonly RNGCryptoServiceProvider _rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        private readonly byte[] _keyBytes;
        private readonly TripleDESCryptoServiceProvider _tripleDES;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5379:Do Not Use Weak Key Derivation Function Algorithm", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "<Pending>")]
        public TripleDESEncryptionService(string password)
        {
            _password = password;
            var passwordBytes = Encoding.UTF8.GetBytes(_password);
            byte[] salt = Encoding.UTF8.GetBytes(_password);
            salt = Enumerable.Range(0, _iterations).Aggregate(salt, (current, i) => _sha1.ComputeHash(current));
            using Rfc2898DeriveBytes passwordDeriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, _iterations);
           _keyBytes = passwordDeriveBytes.GetBytes(_keySize / 8);
           _tripleDES = new TripleDESCryptoServiceProvider();
           _tripleDES.Mode = CipherMode.CBC;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "<Pending>")]
        public Span<byte> Encrypt(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Span<byte> iv = stackalloc byte[16];
            _rngCryptoServiceProvider.GetNonZeroBytes(iv);

            using ICryptoTransform encryptor = _tripleDES.CreateEncryptor(_keyBytes, iv.ToArray());
            using MemoryStream toMemoryStream = new MemoryStream();
            using CryptoStream writer = new CryptoStream(toMemoryStream, encryptor, CryptoStreamMode.Write);
            var valueBytes = data;

            writer.Write(valueBytes, 0, valueBytes.Length);
            writer.FlushFinalBlock();
            var encrypted = new byte[16 + toMemoryStream.Length];
            iv.ToArray().CopyTo(encrypted, 0);
            toMemoryStream.ToArray().CopyTo(encrypted, 16);

            _tripleDES.Clear();
            return new Span<byte>(encrypted);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "<Pending>")]
        public Span<byte> Decrypt(byte[] encryptedData)
        {
            if (encryptedData == null)
            {
                throw new ArgumentNullException(nameof(encryptedData));
            }

            byte[] rawBytes = encryptedData;
            byte[] valueBytes = new byte[rawBytes.Length - 16];
            byte[] ivBytes = new byte[16];

            Array.Copy(rawBytes, 16, valueBytes, 0, rawBytes.Length - 16);
            Array.Copy(rawBytes, 0, ivBytes, 0, 16);

            try
            {
                using ICryptoTransform decryptor = _tripleDES.CreateDecryptor(_keyBytes, ivBytes);
                using MemoryStream fromMemoryStream = new MemoryStream(valueBytes);
                using CryptoStream reader = new CryptoStream(fromMemoryStream, decryptor, CryptoStreamMode.Read);
                var decrypted = new byte[valueBytes.Length];
                var bytesCount = reader.Read(decrypted, 0, decrypted.Length);

                _tripleDES.Clear();
                return new Span<byte>(decrypted).Slice(0, bytesCount);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
