using System;
using Datatent.Core.Pages;

namespace Datatent.Core.Service.Compression
{
    public class UncompressedCompressionService : ICompressionService
    {
        private static readonly Guid IDENTIFIER = Guid.Parse("572C93E1-BB1C-472C-90E2-3EBD0FF42ABE");
        
        public Guid Identifier => IDENTIFIER;
        public byte[] Compress(byte[] bytes)
        {
            return bytes;
        }

        public byte[] Decompress(byte[] bytes)
        {
            return bytes;
        }
    }
}
