using System;
using System.Runtime.InteropServices;
using Datatent.Core.IO;
using Datatent.Core.Service;

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
        [StructLayout(LayoutKind.Explicit, Size = DOCUMENT_HEADER_LENGTH)]
        internal struct DocumentHeader
        {
            /// <summary>
            /// The document id
            /// </summary>
            [FieldOffset(DOCUMENT_ID)]
            public Guid DocumentId;

            /// <summary>
            /// The compressed and actual saved content length
            /// </summary>
            [FieldOffset(DOCUMENT_LENGTH)]
            public uint SavedContentLength;

            /// <summary>
            /// The uncompressed content length
            /// </summary>
            [FieldOffset(DOCUMENT_ORG_LENGTH)]
            public uint OriginalContentLength;
            
            /// <summary>
            /// Indicates whether the document is deleted
            /// </summary>
            [FieldOffset(DOCUMENT_IS_DELETED)]
            public bool IsDeleted;

            /// <summary>
            /// The id that map to the type informations
            /// </summary>
            [FieldOffset(DOCUMENT_TYPE_ID)]
            public uint TypeId;
        }

        /// <summary>
        /// The current memory slice to operate on.
        /// </summary>
        private readonly Memory<byte> _documentSlice;
        
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
        /// Header position of the is deleted flag (byte 24) of type byte
        /// </summary>
        public const byte DOCUMENT_IS_DELETED = 24;

        /// <summary>
        /// Header position of the document type id (byte 25-28) of type uint32
        /// </summary>
        public const int DOCUMENT_TYPE_ID = 25;

        /// <summary>
        /// The length of the header
        /// </summary>
        public const int DOCUMENT_HEADER_LENGTH = 48;
        
        public DocumentHeader Header;

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
            if (memory.IsEmpty || memory.Length < DOCUMENT_HEADER_LENGTH)
                return (null, Guid.Empty);

            if (memory.Span[0] == 0x00)
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
        public Document(Memory<byte>? documentSlice)
        {
            if (documentSlice == null)
                throw new ArgumentNullException(nameof(documentSlice));

            _documentSlice = (Memory<byte>) documentSlice;

            Header = MemoryMarshal.Read<DocumentHeader>(_documentSlice.Span);
        }

        /// <summary>
        /// Creates an empty document, but don't write any data to the memory.
        /// </summary>
        /// <param name="documentSlice"></param>
        /// <param name="id"></param>
        public Document(Memory<byte> documentSlice, Guid id)
        {
            _documentSlice = documentSlice;
            var header = new DocumentHeader();
            header.DocumentId = id;
            header.OriginalContentLength = 0;
            header.SavedContentLength = 0;
            header.IsDeleted = false;
            header.TypeId = 0;
            Header = header;
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
            ref DocumentHeader header = ref Header;
            header.OriginalContentLength = (uint) content.Length;
            
            header.SavedContentLength = (uint) toSave.Length;

            _documentSlice.WriteBytes(DOCUMENT_HEADER_LENGTH, toSave);
            return header.SavedContentLength;
        }

        /// <summary>
        /// Retrieves the current content from the memory.
        /// </summary>
        /// <returns></returns>
        public byte[] GetContent()
        {
            var compressedContent = _documentSlice.Span.ReadBytes(DOCUMENT_HEADER_LENGTH, (int) Header.SavedContentLength);

            return compressedContent;
        }

        /// <summary>
        /// Update the header informations of the document.
        /// </summary>
        /// <param name="typeId"></param>
        public void Update(uint typeId)
        {
            ref DocumentHeader header = ref Header;
            header.TypeId = typeId;
            // write header, content is already set
            MemoryMarshal.Write(_documentSlice.Span, ref header);
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
