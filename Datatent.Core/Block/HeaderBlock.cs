using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Block
{
    internal class HeaderBlock : BaseBlock
    {
        public HeaderBlock(IMemoryOwner<byte> memory) : base(memory)
        {
        }
    }
}
