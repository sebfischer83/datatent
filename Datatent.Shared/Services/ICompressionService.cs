using System;

namespace Datatent.Shared.Services
{
    public interface ICompressionService : IPipelineService
    {
        Span<byte> Compress(Span<byte> bytes);
        Span<byte> Decompress(Span<byte> bytes);
    }
}