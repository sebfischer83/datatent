using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    internal abstract class FileSystemService : IDisposable
    {
        private readonly DatatentSettings _settings;
        private readonly ILogger<FileSystemService> _logger;


        protected FileSystemService(DatatentSettings settings, ILogger<FileSystemService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public abstract void Dispose();
    }
}
