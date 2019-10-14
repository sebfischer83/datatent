using Datatent.Core.Pages;
using K4os.Compression.LZ4;

namespace Datatent.Core.Service.Compression
{
    public class Lz4CompressionService : ICompressionService
    {
        public byte[] Compress(byte[] bytes)
        {
            return LZ4Pickler.Pickle(bytes, LZ4Level.L00_FAST);
        }

        public byte[] Decompress(byte[] bytes)
        {
            return LZ4Pickler.Unpickle(bytes);
        }

        public CompressionType CompressionType => CompressionType.Lz4;
    }
}
