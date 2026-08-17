using ZLinq;
using System.Collections.Generic;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class ZLinqExtensions
    {
        public static IEnumerable<T> AsEnumerable<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> valueEnumerable)
            where TEnumerator : struct, IValueEnumerator<T>
        {
            using var enumerator = valueEnumerable.Enumerator;
            while(enumerator.TryGetNext(out var current))
            {
                yield return current;
            }
        }
    }
}
