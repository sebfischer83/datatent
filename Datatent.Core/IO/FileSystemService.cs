using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Datatent.Core.IO
{
    internal class FileSystemService
    {
        private readonly DatatentSettings _settings;
        private readonly ILogger<FileSystemService> _logger;

        public FileSystemService(DatatentSettings settings, ILogger<FileSystemService> logger)
        {
            _settings = settings;
            _logger = logger;
        }


    }
}
