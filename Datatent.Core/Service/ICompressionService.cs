using Datatent.Core.Pages;

namespace Datatent.Core.Service
{
    internal interface ICompressionService
    {
        byte[] Compress(byte[] bytes, CompressionType compressionType);
        byte[] Decompress(byte[] bytes, CompressionType compressionType);
    }
}