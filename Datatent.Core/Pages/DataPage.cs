using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Datatent.Core.IO;
using Datatent.Core.Service;
using Datatent.Core.Service.Cache;
using Datatent.Core.Service.Compression;
using Datatent.Core.Service.Encryption;

namespace Datatent.Core.Pages
{
    internal class DataPage : BasePage
    {
        public DataPage() : base()
        {
        }

        public (bool Saved, ushort Id) TryAddContent(byte[] content, uint typeId)
        {
            // data are specific to one data type
            if (typeId != Header.PageTypeId)
            {
                return (false, 0);
            }

            var compressedContent = content;
            if (compressedContent.Length > this.Header.PageNumberOfFreeBytes)
            {
                return (false, 0);
            }

            // its enough space in this page
            // now find the place to save
            var sliceIterator = _pageMemorySlice.Slice(0);
            var foundEmptySlot = FindEmptySlot(ref sliceIterator);
            if (!foundEmptySlot)
            {
                return (false, 0);
            }
            Document.Document document = new Document.Document(sliceIterator, this.Header.PageNextDocumentId);
            document.Update(content);

            //this._cachedDocuments.Add(document.DocumentId, document);
            this.IsDirty = true;
            ref PageHeader header = ref Header;
            header.PageNumberOfEntries++;
            header.PageNextDocumentId++;
            header.PageNumberOfFreeBytes = header.PageNumberOfFreeBytes -
                                         (document.Header.ContentLength + Constants.DOCUMENT_HEADER_SIZE);
            return (true, document.Header.DocumentId);
        }

        public async ValueTask<byte[]?> FindByIdAsync(ushort id)
        {
            var result = await Task.Run(() => FindById(id)).ConfigureAwait(false);

            return result;
        }

        public byte[]? FindById(ushort id)
        {
            //if (_cachedDocuments.ContainsKey(id))
            //{
            //    return _cachedDocuments[id].GetContent();
            //}

            foreach (var document in IterateAllDocuments())
            {
                if (document.Header.DocumentId == id)
                {
                    return (document.GetContent());
                }
            }

            return null;
        }

        private IEnumerable<Document.Document> IterateAllDocuments()
        {
            var sliceIterator = _pageMemorySlice.Slice(0);
            var result = Document.Document.GetNextDocumentSliceAndAdjustOffset(ref sliceIterator);
            while (result.DocumentSlice != null)
            {
                Debug.Assert(result.DocumentSlice != null, "result.DocumentSlice != null");
                var doc = new Document.Document(result.DocumentSlice);
                //if (_cachedDocuments.ContainsKey(doc.DocumentId))
                //{
                //    _cachedDocuments.Remove(doc.DocumentId);
                //}
                //_cachedDocuments.Add(doc.DocumentId, doc);

                yield return doc;

                result = Document.Document.GetNextDocumentSliceAndAdjustOffset(ref sliceIterator);
            }
        }

        private bool FindEmptySlot(ref Memory<byte> sliceIterator)
        {
            var result = Document.Document.GetNextDocumentSliceAndAdjustOffset(ref sliceIterator);
            while (result.DocumentSlice != null)
            {
                result = Document.Document.GetNextDocumentSliceAndAdjustOffset(ref sliceIterator);
            }

            return true;
        }
    }
}
