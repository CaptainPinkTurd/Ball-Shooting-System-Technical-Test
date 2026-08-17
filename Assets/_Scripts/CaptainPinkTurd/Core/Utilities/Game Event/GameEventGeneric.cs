using System;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;

namespace CaptainPinkTurd.Core
{
    public class GameEvent<T>
    {
        private readonly SortedDictionary<int, List<Action<T>>> listenersByPriority = new();

        private struct CachedListener
        {
            public readonly Action<T> Callback;
            public readonly EPriority Priority;
            public CachedListener(Action<T> callback, EPriority priority)
            {
                Callback = callback;
                Priority = priority;
            }
        }
        private readonly List<CachedListener> cachedListeners = new();
        // Copy constructor
        public GameEvent(GameEvent<T> other = null)
        {
            if (other == null) return;
            foreach (var pair in other.listenersByPriority)
            {
                listenersByPriority[pair.Key] = new List<Action<T>>(pair.Value);
            }
            cachedListeners = new List<CachedListener>(other.cachedListeners);
        }

        public void Subscribe(Action<T> listener, EPriority priority = EPriority.Medium,
            bool rememberListener = true)
        {
            Unsubscribe(listener); // ensure uniqueness

            int key = (int)priority;
            if (!listenersByPriority.TryGetValue(key, out var list))
            {
                list = new List<Action<T>>();
                listenersByPriority.Add(key, list);
            }

            list.Add(listener);
            if (rememberListener && !cachedListeners.Exists(c => c.Callback == listener))
            {
                cachedListeners.Add(new CachedListener(listener, priority));
            }
        }
        public void Unsubscribe(Action<T> listener)
        {
            foreach (var list in listenersByPriority.Values)
            {
                list.Remove(listener);
            }

            cachedListeners.RemoveAll(c => c.Callback == listener);
        }
        public void Raise(T value)
        {
            var tempListenersPriority = new SortedDictionary<int, List<Action<T>>>(listenersByPriority);
            foreach (var pair in tempListenersPriority)
            {
                var tempListeners = new List<Action<T>>(pair.Value);
                foreach (var listener in tempListeners)
                {
                    listener?.Invoke(value);
                }
            }
        }
        public void Clear()
        {
            listenersByPriority.Clear();
        }
        public void RefreshWithCachedListeners()
        {
            listenersByPriority.Clear();

            var tempCached = new List<CachedListener>(cachedListeners);
            cachedListeners.Clear();

            foreach (var cached in tempCached)
            {
                if (cached.Callback.Target is UnityEngine.Object unityObject && !unityObject)
                    continue;
                Subscribe(cached.Callback, cached.Priority);
            }
        }
    }

    public class GameEvent<T1, T2>
    {
        private readonly SortedDictionary<int, List<Action<T1, T2>>> listenersByPriority = new();

        private struct CachedListener
        {
            public readonly Action<T1, T2> Callback;
            public readonly EPriority Priority;
            public CachedListener(Action<T1, T2> callback, EPriority priority)
            {
                Callback = callback;
                Priority = priority;
            }
        }
        private readonly List<CachedListener> cachedListeners = new();

        public GameEvent(GameEvent<T1, T2> other = null)
        {
            if (other == null) return;
            foreach (var pair in other.listenersByPriority)
            {
                listenersByPriority[pair.Key] = new List<Action<T1, T2>>(pair.Value);
            }
            cachedListeners = new List<CachedListener>(other.cachedListeners);
        }

        public void Subscribe(Action<T1, T2> listener, EPriority priority = EPriority.Medium,
            bool rememberListener = true)
        {
            Unsubscribe(listener);

            int key = (int)priority;
            if (!listenersByPriority.TryGetValue(key, out var list))
            {
                list = new List<Action<T1, T2>>();
                listenersByPriority.Add(key, list);
            }

            list.Add(listener);
            if (rememberListener && !cachedListeners.Exists(c => c.Callback == listener))
            {
                cachedListeners.Add(new CachedListener(listener, priority));
            }
        }

        public void Unsubscribe(Action<T1, T2> listener)
        {
            foreach (var list in listenersByPriority.Values)
            {
                list.Remove(listener);
            }

            cachedListeners.RemoveAll(c => c.Callback == listener);
        }

        public void Raise(T1 value1, T2 value2)
        {
            var tempListenersPriority = new SortedDictionary<int, List<Action<T1, T2>>>(listenersByPriority);
            foreach (var pair in tempListenersPriority)
            {
                var tempListeners = new List<Action<T1, T2>>(pair.Value);
                foreach (var listener in tempListeners)
                {
                    listener?.Invoke(value1, value2);
                }
            }
        }

        public void Clear()
        {
            listenersByPriority.Clear();
        }

        public void RefreshWithCachedListeners()
        {
            listenersByPriority.Clear();

            var tempCached = new List<CachedListener>(cachedListeners);
            cachedListeners.Clear();

            foreach (var cached in tempCached)
            {
                if (cached.Callback.Target is UnityEngine.Object unityObject && !unityObject)
                    continue;
                Subscribe(cached.Callback, cached.Priority);
            }
        }
    }
}
