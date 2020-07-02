//using System;
//using System.Collections.Generic;
//using System.Text;
//using Datatent.Core.Memory;
//using FluentAssertions;
//using Xunit;

//namespace Datatent.Core.Tests.Memory
//{
//    public class NativeByteMemoryPoolTest
//    {
//        [Fact]
//        public void Test()
//        {
//            using var x = new NativeByteMemoryManager(850000);

//            x.Should().NotBeNull();
//            var m = x.Memory;
//            m.Should().NotBeNull();
//        }
//    }
//}
