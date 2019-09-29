using System;
using System.Buffers;

// ReSharper disable ForCanBeConvertedToForeach

namespace Datatent.Core.Algo.Sort
{
    /// <summary>
    /// least significant digit (LSD) radix sort implementation
    /// </summary>
    /// <remarks>
    /// https://en.wikibooks.org/wiki/Algorithm_Implementation/Sorting/Radix_sort
    /// </remarks>
    internal class RadixLds
    {
        private const int BitsPerInt = 32;

        public static void Sort(uint[] a, bool usePool = true)
        {
            // our helper array 
            uint[] t = usePool ? ArrayPool<uint>.Shared.Rent(a.Length) : new uint[a.Length];

            // number of bits our group will be long 
            var r = 4; // try to set this also to 2, 8 or 16 to see if it is quicker or not 

            
            // counting and prefix arrays
            // (note dimensions 2^r which is the number of all possible values of a r-bit number) 
            var count = new int[1 << r];
            var pref = new int[1 << r];
            
            // number of groups 
            int groups = (int)Math.Ceiling(BitsPerInt / (double)r);

            // the mask to identify groups 
            int mask = (1 << r) - 1;

            // the algorithm: 
            for (int c = 0, shift = 0; c < groups; c++, shift += r)
            {
                // reset count array 
                for (int j = 0; j < count.Length; j++)
                    count[j] = 0;

                // counting elements of the c-th group 
                for (int i = 0; i < a.Length; i++)
                {
                    count[(a[i] >> shift) & mask]++;
                }

                // calculating prefixes 
                pref[0] = 0;
                for (int i = 1; i < count.Length; i++)
                {
                    pref[i] = pref[i - 1] + count[i - 1];
                }

                // from a[] to t[] elements ordered by c-th group 
                for (int i = 0; i < a.Length; i++)
                {
                    t[pref[(a[i] >> shift) & mask]++] = a[i];
                }

                // a[]=t[] and start again until the last group 
                Array.Copy(t, a, a.Length);
            }

            if (usePool)
            {
                ArrayPool<uint>.Shared.Return(t);
            }
        }
    }
}
