using System;
using System.Collections.Generic;
using System.Text;

namespace Datatent.Core.Algo.Sort
{
    internal static class Insertion
    {
        public static void Sort<T>(ref T[] array) where T : IComparable<T>
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int j = i + 1;
                while (j > 0)
                {
                    if (array[j - 1].CompareTo(array[j]) > 1)
                    {
                        T temp = array[j - 1];
                        array[j - 1] = array[j];
                        array[j] = temp;
                    }

                    j--;
                }
            }
        }
    }
}
