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
            public ushort DocumentId;

            /// <summary>
            /// The compressed and actual saved content length
            /// </summary>
            [FieldOffset(DOCUMENT_LENGTH)]
            public uint ContentLength;
            
            /// <summary>
            /// Indicates whether the document is deleted
            /// </summary>
            [FieldOffset(DOCUMENT_IS_DELETED)]
            public bool IsDeleted;
        }

        /// <summary>
        /// The current memory slice to operate on.
        /// </summary>
        private readonly Memory<byte> _documentSlice;
        
        /// <summary>
        /// Header position of the document id (byte 0-1) of type ushort
        /// </summary>
        public const int DOCUMENT_ID = 0;

        /// <summary>
        /// Header position of the document length without the header (byte 2-5) of type uint32
        /// </summary>
        public const int DOCUMENT_LENGTH = 2;

        /// <summary>
        /// Header position of the uncompressed document length without the header (byte 6-10) of type uint32
        /// </summary>
        public const int DOCUMENT_ORG_LENGTH = 6;
        
        /// <summary>
        /// Header position of the is deleted flag (byte 11) of type byte
        /// </summary>
        public const byte DOCUMENT_IS_DELETED = 11;

        /// <summary>
        /// Header position of the document type id (byte 12-15) of type uint32
        /// </summary>
        public const int DOCUMENT_TYPE_ID = 12;

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
        public static (Memory<byte>? DocumentSlice, ushort DocumentId) GetNextDocumentSliceAndAdjustOffset(ref Memory<byte> memory)
        {
            if (memory.IsEmpty || memory.Length < DOCUMENT_HEADER_LENGTH)
                return (null, 0);

            if (memory.Span[0] == 0x00)
                return (null, 0);

            var id = memory.Span.ReadUInt16(DOCUMENT_ID);
            
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
        public Document(Memory<byte> documentSlice, ushort id)
        {
            _documentSlice = documentSlice;
            var header = new DocumentHeader();
            header.DocumentId = id;
            header.ContentLength = 0;
            header.IsDeleted = false;
            Header = header;
        }


        /// <summary>
        /// Set the content to the memory but don't update the header.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public void SetContent(byte[] content)
        {
            var toSave = content;
            ref DocumentHeader header = ref Header;
            
            header.ContentLength = (uint) toSave.Length;

            _documentSlice.WriteBytes(DOCUMENT_HEADER_LENGTH, toSave);
        }

        /// <summary>
        /// Retrieves the current content from the memory.
        /// </summary>
        /// <returns></returns>
        public byte[] GetContent()
        {
            var compressedContent = _documentSlice.Span.ReadBytes(DOCUMENT_HEADER_LENGTH, (int) Header.ContentLength);

            return compressedContent;
        }

        /// <summary>
        /// Update the header informations of the document.
        /// </summary>
        public void UpdateHeader()
        {
            ref DocumentHeader header = ref Header;
            // write header, content is already set
            MemoryMarshal.Write(_documentSlice.Span, ref header);
        }

        /// <summary>
        /// Update the header and the content of the document.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public void Update(byte[] content)
        {
            this.SetContent(content);
            this.UpdateHeader();
        }
    }
}
