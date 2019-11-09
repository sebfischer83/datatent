using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Algo.Sort;
using Datatent.Core.Common;
using Datatent.Core.IO;
using Datatent.Core.Scheduler;
using Datatent.Core.Service;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using Microsoft.Extensions.Logging;

namespace Datatent.Core
{
    public interface IDatabase
    {
        void Insert<T>(T item);
        void Delete<T>(T item);
        void Update<T>(T item);
        void Get<T>();
        void Find<T, TKey>(TKey id);
    }

    public class Database
    {
        private readonly DatatentSettings _settings;
        private readonly IDataProcessingPipeline _processingPipeline;
        private readonly DefaultScheduler _defaultScheduler;
        private readonly FileSystemService _fileSystemService;

        public static Database CreateNew(ILoggerFactory loggerFactory, string path, string password = "")
        {
            DatatentSettings settings = new DatatentSettings();
            settings.DataFile = path;
            IDataProcessingPipeline processingPipeline = new DataProcessingPipeline(new Lz4CompressionService(),
                string.IsNullOrWhiteSpace(password) ? (IEncryptionService) new NullEncryptionService() : new AESEncryptionService(password));

            var database = new Database(loggerFactory, settings, processingPipeline);
            database.InitNew();
            
            return database;
        }

        public Database(ILoggerFactory loggerFactory, DatatentSettings settings, IDataProcessingPipeline processingPipeline)
        {
            _settings = settings;
            _processingPipeline = processingPipeline;

            _fileSystemService = new FileSystemService(_settings, loggerFactory.CreateLogger<FileSystemService>());
            _defaultScheduler = new DefaultScheduler(_fileSystemService);
        }

        private void InitNew()
        {
            var header = new DatabaseHeader(Constants.VERSION, _processingPipeline.GetInformations());
            _fileSystemService.WriteDatabaseHeader(ref header);
        }
    }
}
