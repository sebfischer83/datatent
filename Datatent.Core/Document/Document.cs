using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Datatent.Core.IO;
using Datatent.Core.Pages;
using Datatent.Core.Service;
using K4os.Compression.LZ4;

namespace Datatent.Core.Document
{
    /// <summary>
    /// Documents hold the actual data and are the smallest part of a data file.
    /// </summary>
    /// <remarks>
    /// A page can contain multiple documents of different types. The type of the object is mapped in the header pages.
    /// </remarks>
    internal class Document
    {
        /// <summary>
        /// The current memory slice to operate on.
        /// </summary>
        private readonly Memory<byte> _documentSlice;

        private readonly ICompressionService _compressionService;
        
        /// <summary>
        /// Header position of the document id (byte 0-15) of type guid
        /// </summary>
        public const int DOCUMENT_ID = 0;

        /// <summary>
        /// Header position of the document length without the header (byte 16-19) of type uint32
        /// </summary>
        public const int DOCUMENT_LENGTH = 16;

        /// <summary>
        /// Header position of the uncompressed document length without the header (byte 20-23) of type uint32
        /// </summary>
        public const int DOCUMENT_ORG_LENGTH = 20;

        /// <summary>
        /// Header position of the compression type (byte 24) of type byte
        /// </summary>
        public const byte DOCUMENT_COMPRESSION_TYPE = 24;

        /// <summary>
        /// Header position of the is deleted flag (byte 25) of type byte
        /// </summary>
        public const byte DOCUMENT_IS_DELETED = 25;

        /// <summary>
        /// Header position of the document type id (byte 26-29) of type uint32
        /// </summary>
        public const int DOCUMENT_TYPE_ID = 26;

        /// <summary>
        /// The length of the header
        /// </summary>
        public const int DOCUMENT_HEADER_LENGTH = 48;

        /// <summary>
        /// The document id
        /// </summary>
        public Guid DocumentId { get; private set; }

        /// <summary>
        /// The compression type
        /// </summary>
        /// <see cref="CompressionType"/>
        public CompressionType CompressionType { get; private set; }
        
        /// <summary>
        /// The uncompressed content length
        /// </summary>
        public uint OriginalContentLength { get; private set; }

        /// <summary>
        /// The compressed and actual saved content length
        /// </summary>
        public uint SavedContentLength { get; private set; }

        /// <summary>
        /// Holds temporary the compressed content
        /// </summary>
        protected byte[]? _compressedContent;

        /// <summary>
        /// Indicates whether the document is deleted
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// The id that map to the type informations
        /// </summary>
        public uint TypeId { get; private set; }

        /// <summary>
        /// Gets the memory slice from the beginning until the end of the document and adjust the offset of the given memory slice to the next document.
        /// </summary>
        /// <param name="memory">the memory slice that holds the document</param>
        /// <remarks>
        /// Assumes that the document starts at index 0 of the memory slice.
        /// </remarks>
        /// <returns></returns>
        public static (Memory<byte>? DocumentSlice, Guid DocumentId) GetNextDocumentSliceAndAdjustOffset(ref Memory<byte> memory)
        {
            if (memory.IsEmpty || memory.Length < DOCUMENT_HEADER_LENGTH || memory.Span.ReadByte(0) == 0x00)
                return (null, Guid.Empty);

            var id = memory.Span.ReadGuid(DOCUMENT_ID);
            var docLength = memory.Span.ReadUInt32(DOCUMENT_LENGTH);
            var docSlice = memory.Slice(0, (int) (docLength + DOCUMENT_HEADER_LENGTH));

            memory = memory.Slice((int) (docLength + DOCUMENT_HEADER_LENGTH));

            return (docSlice, id);
        }

        /// <summary>
        /// Construct the document header from the given memory slice.
        /// </summary>
        /// <param name="documentSlice"></param>
        /// <param name="compressionService"></param>
        public Document(Memory<byte> documentSlice, ICompressionService compressionService)
        {
            _documentSlice = documentSlice;
            _compressionService = compressionService;
            DocumentId = documentSlice.Span.ReadGuid(DOCUMENT_ID);
            SavedContentLength = documentSlice.Span.ReadUInt32(DOCUMENT_LENGTH);
            OriginalContentLength = documentSlice.Span.ReadUInt32(DOCUMENT_ORG_LENGTH);
            CompressionType = (CompressionType) documentSlice.Span.ReadByte(DOCUMENT_COMPRESSION_TYPE);
            IsDeleted = documentSlice.Span.ReadBool(DOCUMENT_IS_DELETED);
            TypeId = documentSlice.Span.ReadUInt32(DOCUMENT_TYPE_ID);
        }

        /// <summary>
        /// Creates an empty document, but don't write any data to the memory.
        /// </summary>
        /// <param name="documentSlice"></param>
        /// <param name="id"></param>
        /// <param name="compressionService"></param>
        public Document(Memory<byte> documentSlice, Guid id, ICompressionService compressionService)
        {
            _documentSlice = documentSlice;
            _compressionService = compressionService;
            DocumentId = id;
            OriginalContentLength = 0;
            SavedContentLength = 0;
            CompressionType = Constants.COMPRESSION_TYPE;
            IsDeleted = false;
            TypeId = 0;
        }

        /// <summary>
        /// Checks how much space is needed for the given content.
        /// </summary>
        /// <param name="content">The content that should be saved.</param>
        /// <returns>The needed space.</returns>
        public uint CheckNeededSpace(byte[] content)
        {
            if (_compressedContent == null)
            {
                _compressedContent = _compressionService.Compress(content, CompressionType);
            }

            return (uint) _compressedContent.Length;
        }

        /// <summary>
        /// Set the content to the memory but don't update the header.
        /// </summary>
        /// <param name="content"></param>
        /// <see cref="Update(uint)"/>
        /// <returns></returns>
        public uint SetContent(byte[] content)
        {
            var toSave = content;
            OriginalContentLength = (uint) content.Length;
            toSave = _compressedContent ?? _compressionService.Compress(toSave, CompressionType);

            SavedContentLength = (uint) toSave.Length;

            _documentSlice.Span.WriteBytes(DOCUMENT_HEADER_LENGTH, toSave);

            _compressedContent = null;
            return SavedContentLength;
        }

        /// <summary>
        /// Retrieves the current content from the memory.
        /// </summary>
        /// <returns></returns>
        public byte[] GetContent()
        {
            var compressedContent = _documentSlice.Span.ReadBytes(DOCUMENT_HEADER_LENGTH, (int) SavedContentLength);

            return _compressionService.Decompress(compressedContent, CompressionType);
        }

        /// <summary>
        /// Update the header informations of the document.
        /// </summary>
        /// <param name="typeId"></param>
        public void Update(uint typeId)
        {
            this.TypeId = typeId;
            // write header, content is already set
            _documentSlice.Span.WriteGuid(DOCUMENT_ID, DocumentId);
            _documentSlice.Span.WriteUInt32(DOCUMENT_LENGTH, SavedContentLength);
            _documentSlice.Span.WriteUInt32(DOCUMENT_ORG_LENGTH, OriginalContentLength);
            _documentSlice.Span.WriteByte(DOCUMENT_COMPRESSION_TYPE, (byte) CompressionType);
            _documentSlice.Span.WriteBool(DOCUMENT_IS_DELETED, IsDeleted);
            _documentSlice.Span.WriteUInt32(DOCUMENT_TYPE_ID, TypeId);
        }

        /// <summary>
        /// Update the header and the content of the document.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public uint Update(byte[] content, uint typeId)
        {
            var length = this.SetContent(content);

            Update(typeId);
            return length;
        }
    }
}
