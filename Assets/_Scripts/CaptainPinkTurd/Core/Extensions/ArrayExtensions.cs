using System;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Copies a specified number of elements from one array to another, starting at the given offsets.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if the source or destination array is null.</exception>
        /// <example>
        /// Example Usage: 
        /// <code>
        /// int[] sourceArray = { 1, 2, 3, 4, 5 };
        /// int[] destinationArray = new int[6];
        /// sourceArray.BlockCopy(1, destinationArray, 2, 3);
        /// // destinationArray now contains: { 0, 0, 2, 3, 4, 0 }
        /// </code>
        /// </example>
        public static void BlockCopy<T>(this T[] source, int sourceOffset, T[] destination, int destinationOffset, int count)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            if(destination == null) throw new ArgumentNullException(nameof(destination));
            
            source.AsSpan().BlockCopy(sourceOffset, destination.AsSpan(), destinationOffset, count);
        }
        
        /// <summary>
        /// Copies a specified number of elements from one span to another, starting at the given offsets.
        /// Provides a low-level and efficient way to copy between spans.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if the source or destination does not have enough space for the specified count starting at the given offsets.
        /// </exception>
        public static void BlockCopy<T>(this Span<T> source, int sourceOffset, 
            Span<T> destination, int destinationOffset, int count)
        {
            if((uint)(sourceOffset + count) > (uint)source.Length) 
                throw new ArgumentException("Source span is too small");
            if((uint)(destinationOffset + count) > (uint)destination.Length) 
                throw new ArgumentException("Destination span is too small");
            
            source.Slice(sourceOffset, count).CopyTo(destination.Slice(destinationOffset, count));
        }
    }
}