using System;
using System.Collections.Generic;
using System.Text;
using Datatent.Core.Algo.Sort;
using FluentAssertions;
using Xunit;

namespace Datatent.Core.Tests.Algo.Sort
{
    public class RadixLdsUnsafeTest
    {
        [Fact]
        public void SortTest()
        {
            const int listLength = 32000;

            var list = new uint[listLength];
            var listStuff = new uint[listLength];
            Random random = new Random();
            
            for (int i = 0; i < listLength; i++)
            {
                list[i] = (uint) random.Next(0, int.MaxValue);
                listStuff[i] = 0;
            }

            var compareList = new uint[listLength];
            
            Array.Copy(list, compareList, listLength);

            Array.Sort(compareList);
            list.AsSpan().Sort(listStuff.AsSpan(), 0, false);

            list.Should().BeEquivalentTo(compareList);
        }
    }
}
