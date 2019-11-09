using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.Service;

namespace Datatent.Core.Common
{
    [StructLayout(LayoutKind.Explicit, Size = Constants.DATABASE_HEADER_SIZE)]
    internal readonly struct DatabaseHeader
    {
        [FieldOffset(0)]
        public readonly char Identifier;

        [FieldOffset(2)]
        public readonly ushort Version;

        [FieldOffset(4)]
        public readonly DataProcessingInformations DataProcessingInformations;

        public DatabaseHeader(ushort version, DataProcessingInformations dataProcessingInformations)
        {
            Identifier = 'D';
            Version = version;
            DataProcessingInformations = dataProcessingInformations;
        }
    }
}
