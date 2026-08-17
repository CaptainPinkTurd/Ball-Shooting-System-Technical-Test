using System;

namespace CaptainPinkTurd.Core.Utils
{
    public static class ArrayUtils
    {
        private static readonly System.Random _rng = new System.Random();
        
        /// <summary>
        /// Generates a random int array from minNumber to totalIndex, shuffled.
        /// </summary>
        public static int[] GenerateRandomIntArray(int totalIndex, int minNumber)
        {
            var array = new int[totalIndex];    

            // Fill array with range: [minNumber, minNumber + totalIndex - 1]
            for (int i = 0; i < totalIndex; ++i)
            {
                array[i] = minNumber + i;
            }

            // Span<T> is like a slice or window over data. 
            // It doesn't allocate memory — it just views existing memory, USEFUL for modifying arrays without copying.
            // It's stack-allocated, fast, and can be used to manipulate or read data efficiently.
            Span<int> span = array;

            for (int i = span.Length - 1; i > 0; i--)
            {
                int j = _rng.Next(0, i + 1); // inclusive range
                (span[i], span[j]) = (span[j], span[i]);
            }

            return array;
        }
        
        /// <summary>
        /// Shuffles an existing array in place (Fisher–Yates).
        /// </summary>
        public static void ShuffleInPlace<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }
}
