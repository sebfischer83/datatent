using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Datatent.Core.IO;
using FluentAssertions;

namespace Datatent.Core.Tests.IO
{
    public class SpanExtensionsTest
    {
        [Fact]
        public void GetValidGuidTest()
        {
            Guid expected = Guid.NewGuid();
            Span<byte> span = expected.ToByteArray();

            var guid = span.ReadGuid(0);

            guid.Should().Be(expected);
        }

        [Fact]
        public void GetBytesTest()
        {
            string s = "askljr3489ur34fojirejdresg";
            byte[] testArray = Encoding.UTF8.GetBytes(s);

            Memory<byte> target = new byte[500];
            SpanExtensions.WriteBytes(target, 0, testArray);

            var span = target.Span;
            var bytes = span.ReadBytes(0, testArray.Length);
            var result = Encoding.UTF8.GetString(bytes);

            result.Should().Be(s);
        }
    }
}
