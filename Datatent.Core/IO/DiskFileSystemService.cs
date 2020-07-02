using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Datatent.Core.Scheduler;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    /// <summary>
    /// Uses a file for the data.
    /// </summary>
    /// <seealso cref="Datatent.Core.IO.FileSystemServiceBase" />
    internal sealed class DiskFileSystemService : FileSystemServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiskFileSystemService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public DiskFileSystemService(DatatentSettings settings, ILogger<FileSystemServiceBase> logger) : base(settings, logger)
        {
            _dataFileStream = new FileStream(settings.DataFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.RandomAccess);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            _dataFileStream.Dispose();
        }
    }
}
