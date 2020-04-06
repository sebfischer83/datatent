using System;

namespace Datatent.Core.Algo.Sort
{
    /// <summary>
    /// Simple insertion sort implementation
    /// </summary>
    internal static class Insertion
    {
        /// <summary>
        /// Sorts the specified array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        public static void Sort<T>(ref T[] array) where T : IComparable<T>
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int j = i + 1;
                while (j > 0)
                {
                    if (array[j - 1].CompareTo(array[j]) > 1)
                    {
                        var temp = array[j - 1];
                        array[j - 1] = array[j];
                        array[j] = temp;
                    }

                    j--;
                }
            }
        }
    }
}
