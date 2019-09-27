using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Pages;
using K4os.Compression.LZ4;

namespace Datatent.Core.Service
{
    internal class CompressionService : ICompressionService
    {
        public byte[] Compress(byte[] bytes, CompressionType compressionType)
        {
            switch (compressionType)
            {
                case CompressionType.None:
                    return bytes;
                case CompressionType.Lz4:
                    return LZ4Pickler.Pickle(bytes, LZ4Level.L00_FAST);
                case CompressionType.Gzip:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(compressionType), compressionType, null);
            }
        }

        public byte[] Decompress(byte[] bytes, CompressionType compressionType)
        {
            switch (compressionType)
            {
                case CompressionType.None:
                    return bytes;
                case CompressionType.Lz4:
                    return LZ4Pickler.Unpickle(bytes);
                case CompressionType.Gzip:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(compressionType), compressionType, null);
            }
        }
    }
}
