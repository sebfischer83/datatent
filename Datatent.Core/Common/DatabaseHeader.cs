using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Datatent.Core.Service;
using Datatent.Shared;
using Datatent.Shared.Pipeline;
using Datatent.Shared.Services;

namespace Datatent.Core.Common
{
    /// <summary>
    /// The database header
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = Constants.DATABASE_HEADER_SIZE)]
    internal readonly struct DatabaseHeader
    {
        /// <summary>
        /// The identifier
        /// </summary>
        [FieldOffset(0)]
        public readonly char Identifier;

        /// <summary>
        /// The version
        /// </summary>
        [FieldOffset(2)]
        public readonly ushort Version;

        /// <summary>
        /// Which pipelines are included to create the database
        /// </summary>
        [FieldOffset(4)]
        public readonly DataProcessingInformations DataProcessingInformations;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHeader"/> struct.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="dataProcessingInformations">The data processing informations.</param>
        public DatabaseHeader(ushort version, DataProcessingInformations dataProcessingInformations)
        {
            Identifier = 'D';
            Version = version;
            DataProcessingInformations = dataProcessingInformations;
        }
    }
}
