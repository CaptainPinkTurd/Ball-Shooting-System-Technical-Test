using System.Collections.Generic;
using ZLinq;

namespace CaptainPinkTurd.Core.Enum
{
    public static class EnumUtils 
    {
        public static IEnumerable<T> GetValues<T>() {
            return System.Enum.GetValues(typeof(T)).AsValueEnumerable().Cast<T>().ToArray();
        }
    }
}