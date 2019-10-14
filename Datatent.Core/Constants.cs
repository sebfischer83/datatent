using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Pages;
#pragma warning disable CA1802 // Use literals where appropriate

namespace Datatent.Core
{
    internal sealed class Constants
    {
        public static readonly uint PAGE_SIZE = 64000;

        public const int PAGE_HEADER_SIZE = 92;

        public static readonly uint PAGE_SIZE_INCL_HEADER = PAGE_HEADER_SIZE + PAGE_SIZE;

        public static readonly uint PAGE_PER_BLOCK = 200;

        public static readonly uint BLOCK_SIZE = PAGE_PER_BLOCK * PAGE_SIZE_INCL_HEADER;

        public const int BLOCK_HEADER_SIZE = 92;

        public static readonly uint BLOCK_SIZE_INCL_HEADER = BLOCK_HEADER_SIZE + BLOCK_SIZE;

        public static readonly CompressionType COMPRESSION_TYPE = CompressionType.Lz4;

        public static readonly uint DOCUMENT_HEADER_SIZE = 32;
    }
}
#pragma warning restore CA1802 // Use literals where appropriate
