using System;
using Datatent.Core.Pages;

namespace Datatent.Core.Service.Compression
{
    public interface ICompressionService
    {
        Guid Identifier { get; }

        byte[] Compress(byte[] bytes);
        byte[] Decompress(byte[] bytes);
    }
}