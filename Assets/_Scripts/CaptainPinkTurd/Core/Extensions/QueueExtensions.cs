using System.Collections.Generic;
using ZLinq;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class QueueExtensions
    {
        public static void Remove<T>(this Queue<T> queue, T itemToRemove)
        {
            if (queue == null) return;

            // Rebuild the queue without the unwanted item
            Queue<T> tempQueue = new Queue<T>(queue.AsValueEnumerable()
                .Where(item => !item.Equals(itemToRemove)).AsEnumerable());

            // Clear the original queue and refill it with filtered items
            queue.Clear();
            foreach (var item in tempQueue)
            {
                queue.Enqueue(item);
            }
        }
    }
}
