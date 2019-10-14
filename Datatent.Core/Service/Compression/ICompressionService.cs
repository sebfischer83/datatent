using Datatent.Core.Pages;

namespace Datatent.Core.Service.Compression
{
    public interface ICompressionService
    {
        CompressionType CompressionType { get; }

        byte[] Compress(byte[] bytes);
        byte[] Decompress(byte[] bytes);
    }
}