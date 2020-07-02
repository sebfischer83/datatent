//using System;
//using System.Collections.Generic;
//using System.Text;
//using Datatent.Core.Memory;
//using FluentAssertions;
//using Xunit;

//namespace Datatent.Core.Tests.Memory
//{
//    public class DatatentMemoryPoolTest
//    {
//        [Fact]
//        public void TestRentAndDispose()
//        {
//            var x = ByteMemoryPool.Shared.Rent(500);
//            x.Memory.Length.Should().Be(500);
//            x.Dispose();

//            Func<int> a = () => x.Memory.Length;
//            a.Should().Throw<ObjectDisposedException>();
//        }
//    }
//}
