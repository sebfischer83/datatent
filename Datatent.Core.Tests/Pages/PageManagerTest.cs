using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Datatent.Core.Tests.Pages
{
    public class PageManagerTest
    {
        private Memory<byte> GenerateTestPages()
        {
            // generate test pages
            byte[] testArray = new byte[Constants.PAGE_SIZE_INCL_HEADER * 5];

            Memory<byte> memory = new Memory<byte>(testArray);
            
            // 

            return memory;
        }

        [Fact]
        public void GetPageTest()
        {
            

        }
    }
}
