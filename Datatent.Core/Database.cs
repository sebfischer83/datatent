using Datatent.Core.Common;
using Datatent.Core.IO;
using Datatent.Core.Scheduler;
using Datatent.Core.Service;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;
using Datatent.Core.Service.Serialization;
using Datatent.Shared;
using Datatent.Shared.Pipeline;
using Datatent.Shared.Services;
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
        private readonly DiskFileSystemService _diskFileSystemService;

        public static Database CreateNew(ILoggerFactory loggerFactory, string path, string password = "")
        {
            DatatentSettings settings = new DatatentSettings();
            settings.DataFile = path;
            IDataProcessingPipeline processingPipeline = new DataProcessingPipeline(new UTF8JSonSerializer(), string.IsNullOrWhiteSpace(password) ? (IEncryptionService)new NullEncryptionService() : new AESEncryptionService(password), new Lz4CompressionService());

            var database = new Database(loggerFactory, settings, processingPipeline);
            database.InitNew();
            
            return database;
        }

        public Database(ILoggerFactory loggerFactory, DatatentSettings settings, IDataProcessingPipeline processingPipeline)
        {
            _settings = settings;
            _processingPipeline = processingPipeline;

            _diskFileSystemService = new DiskFileSystemService(_settings, loggerFactory.CreateLogger<DiskFileSystemService>());
            _defaultScheduler = new DefaultScheduler(_diskFileSystemService);
        }

        private void InitNew()
        {
            var header = new DatabaseHeader(Constants.VERSION, _processingPipeline.GetInformations());
            _diskFileSystemService.WriteDatabaseHeader(ref header);
        }
    }
}
