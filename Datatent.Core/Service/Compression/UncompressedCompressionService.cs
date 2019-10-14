using Datatent.Core.Pages;

namespace Datatent.Core.Service.Compression
{
    public class UncompressedCompressionService : ICompressionService
    {
        public CompressionType CompressionType => CompressionType.None;
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
