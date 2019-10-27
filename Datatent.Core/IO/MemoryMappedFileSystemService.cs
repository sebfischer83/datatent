//using System;
//using System.Collections.Generic;
//using System.IO.MemoryMappedFiles;
//using System.Text;
//using Microsoft.Extensions.Logging;

//namespace Datatent.Core.IO
//{
//    internal class MemoryMappedFileSystemService : FileSystemService
//    {
//        private MemoryMappedFile _memoryMappedFile;

//        public MemoryMappedFileSystemService(DatatentSettings settings, ILogger<FileSystemService> logger) : base(settings, logger)
//        {
//            _memoryMappedFile = MemoryMappedFile.CreateOrOpen(settings.DataFile, 1024 * 1024 * 500, MemoryMappedFileAccess.ReadWrite);

//        }

//        public override void Dispose()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
