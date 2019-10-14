using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Service.Encryption;
using FluentAssertions;
using Xunit;

namespace Datatent.Core.Tests.Services
{
    public class TripleDESEncryptionServiceTest
    {
        [Fact]
        public void EncryptDecryptTest()
        {
            string testContent = "Hello World!!!";
            var bytesContent = Encoding.UTF8.GetBytes(testContent);

            TripleDESEncryptionService encryptionService = new TripleDESEncryptionService("testpwd");

            var encrypted = encryptionService.Encrypt(bytesContent);
            var decrypted = encryptionService.Decrypt(encrypted.ToArray());
            var str = Encoding.UTF8.GetString(decrypted);
            str.Should().Be(testContent);
        }
    }
}
