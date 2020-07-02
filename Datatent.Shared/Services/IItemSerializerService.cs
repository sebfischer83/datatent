using System;
using System.Threading.Tasks;

namespace Datatent.Shared.Services
{
    public interface IItemSerializerService : IPipelineService
    {
        Span<byte> Serialize(object item);
        object Deserialize(Span<byte> item);
    }
}
