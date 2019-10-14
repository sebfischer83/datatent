using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Service.Cache
{
    internal class DummyDocumentCacheService : ICacheService<Document.Document>
    {
        public void Cache(string Id, Document.Document document)
        {
            // nop
        }

        public Document.Document Retrieve(string Id)
        {
            return null;
        }

        public bool IsKeyInCache(string Id)
        {
            return false;
        }
    }
}
