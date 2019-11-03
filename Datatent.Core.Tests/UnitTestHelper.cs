using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Datatent.Core.Pages;
using Datatent.Core.IO;
[assembly: InternalsVisibleTo("Datatent.Core.Benchmarks")]

namespace Datatent.Core.Tests
{
    public static class UnitTestHelper
    {
        public static byte[] RandomData (int length)
        {
            var data = new byte[length];
            var rnd = new Random ();
            for (var i = 0; i < data.Length; i++) {
                data[i] = (byte)rnd.Next (0, 256);
            }
            return data;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public static void FillArray(ref byte[] array, byte value)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        internal static byte[] GetPageHeader(uint id, PageType pageType)
        {
            byte[] headerBytes = new byte[Constants.PAGE_HEADER_SIZE];

            var arraySlice = new Memory<byte>(headerBytes);

            arraySlice.Span.WriteUInt32(BasePage.PAGE_ID, id);
            arraySlice.Span.WriteByte(BasePage.PAGE_TYPE, (byte) pageType);
            arraySlice.Span.WriteUInt32(BasePage.PAGE_NEXT_ID, uint.MaxValue);
            arraySlice.Span.WriteUInt32(BasePage.PAGE_PREV_ID, uint.MaxValue);
            arraySlice.Span.WriteUInt32(BasePage.PAGE_NUMBER_OF_ENTRIES, 0);
            arraySlice.Span.WriteUInt32(BasePage.PAGE_NEXT_DOCUMENT_ID, id + 1);
            arraySlice.Span.WriteUInt32(BasePage.PAGE_NUMBER_OF_FREE_BYTES, Constants.PAGE_SIZE);

            return headerBytes;
        }
    }
}
