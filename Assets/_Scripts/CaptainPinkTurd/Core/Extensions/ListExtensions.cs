using System.Collections.Generic;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class ListExtensions {
        public static void RefreshWith<T>(this List<T> list, IEnumerable<T> items) 
        {
            list.Clear();
            list.AddRange(items);
        }
        public static bool TryAdd<T>(this List<T> list, T item, bool allowDuplicate = true)
        {
            // Only block nulls if T is a reference type or Nullable<T>
            if ((item == null && default(T) == null) ||
                (list.Contains(item) && !allowDuplicate)) return false;

            list.Add(item);
            return true;
        }
    }
}