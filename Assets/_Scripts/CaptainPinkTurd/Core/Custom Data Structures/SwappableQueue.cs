using System;
using System.Collections.Generic;

namespace CaptainPinkTurd.Core.CustomDataStructure
{
    public class SwappableQueue<T>
    {
        private readonly List<T> list = new List<T>();

        public SwappableQueue() { }
        
        public SwappableQueue(SwappableQueue<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other), "Cannot copy from a null SwappableQueue.");
            }
            list = new List<T>(other.list);
        }

        public void Enqueue(T item) => list.Add(item);

        public T Dequeue()
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            T first = list[0];
            list.RemoveAt(0);
            return first;
        }

        public T Peek()
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return list[0];
        }

        public T PeekAtIndex(int index)
        {
            if (index < 0 || index >= list.Count)
            {
                throw new ArgumentOutOfRangeException("Index out of bounds.");
            }

            return list[index];
        }

        public int Count => list.Count;

        public void Swap(int indexA, int indexB)
        {
            if (indexA == indexB)
                return;

            if (indexA < 0 || indexB < 0 || indexA >= list.Count || indexB >= list.Count)
            {
                throw new ArgumentOutOfRangeException("Index out of bounds.");
            }

            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }

        public void Clear() => list.Clear();

        public IEnumerable<T> AsEnumerable() => list;
    }
}