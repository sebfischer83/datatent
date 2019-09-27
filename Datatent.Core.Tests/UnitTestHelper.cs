using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
