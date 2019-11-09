using System;
using FluentAssertions;
using Xunit;

namespace Datatent.Core.Tests.Algo.Sort
{
    public class RadixLdsTest
    {
        [Fact]
        public void SortTest()
        {
            const int listLength = 32000;

            var list = new uint[listLength];
            Random random = new Random();
            
            for (int i = 0; i < listLength; i++)
            {
                list[i] = (uint) random.Next(0, int.MaxValue);
            }

            var compareList = new uint[listLength];
            Array.Copy(list, compareList, listLength);

            Array.Sort(compareList);
            Datatent.Core.Algo.Sort.RadixLds.Sort(list);

            list.Should().BeEquivalentTo(compareList);
        }

        [Fact]
        public void SortSpanTest()
        {
            const int listLength = 32000;

            var list = new uint[listLength];
            Random random = new Random();
            
            for (int i = 0; i < listLength; i++)
            {
                list[i] = (uint) random.Next(0, int.MaxValue);
            }

            var compareList = new uint[listLength];
            Array.Copy(list, compareList, listLength);

            Array.Sort(compareList);
            var span = new Span<uint>(list);
            Datatent.Core.Algo.Sort.RadixLds.Sort(span);

            span.ToArray().Should().BeEquivalentTo(compareList);
        }
    }
}
