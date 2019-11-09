using System;
using Datatent.Core.Pages;
using K4os.Compression.LZ4;

namespace Datatent.Core.Service.Compression
{
    public class Lz4CompressionService : ICompressionService
    {
        private static readonly Guid IDENTIFIER = Guid.Parse("DEC89979-AE47-40E0-ABDB-3D2278F8482B");
        
        public Guid Identifier => IDENTIFIER;

        public byte[] Compress(byte[] bytes)
        {
            return LZ4Pickler.Pickle(bytes, LZ4Level.L00_FAST);
        }

        public byte[] Decompress(byte[] bytes)
        {
            return LZ4Pickler.Unpickle(bytes);
        }
    }
}
