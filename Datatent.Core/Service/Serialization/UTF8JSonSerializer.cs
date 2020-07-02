using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Shared.Services;

namespace Datatent.Core.Service.Serialization
{
    public class UTF8JSonSerializer : IItemSerializerService
    {
        private static readonly Guid IDENTIFIER = Guid.Parse("8A508B34-7E79-45B0-944E-F10FD956F9A1");
        public Guid Identifier => IDENTIFIER;
        public Span<byte> Serialize(object item)
        {
            return Utf8Json.JsonSerializer.Serialize(item);
        }

        public object Deserialize(Span<byte> item)
        {
            return Utf8Json.JsonSerializer.Deserialize<object>(item.ToArray());
        }
    }
}
