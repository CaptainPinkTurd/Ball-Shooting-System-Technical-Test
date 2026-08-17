using System;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;

namespace CaptainPinkTurd.Core
{
    public class GameEvent
    {
        private readonly SortedDictionary<int, List<Action>> listenersByPriority = new();

        private struct CachedListener
        {
            public readonly Action Callback;
            public readonly EPriority Priority;

            public CachedListener(Action callback, EPriority priority)
            {
                Callback = callback;
                Priority = priority;
            }
        }

        private readonly List<CachedListener> cachedListeners = new();

        // Copy constructor
        public GameEvent(GameEvent other = null)
        {
            if (other == null) return;

            foreach (var pair in other.listenersByPriority)
            {
                listenersByPriority[pair.Key] = new List<Action>(pair.Value);
            }

            cachedListeners = new List<CachedListener>(other.cachedListeners);
        }
        
        public void Subscribe(Action listener, EPriority priority = EPriority.Medium, 
            bool rememberListener = true)
        {
            Unsubscribe(listener); // ensure uniqueness

            int key = (int)priority;

            if (!listenersByPriority.TryGetValue(key, out var list))
            {
                list = new List<Action>();
                listenersByPriority.Add(key, list);
            }

            list.Add(listener);

            if (rememberListener && !cachedListeners.Exists(c => c.Callback == listener))
            {
                cachedListeners.Add(new CachedListener(listener, priority));
            }
        }

        public void Unsubscribe(Action listener)
        {
            foreach (var list in listenersByPriority.Values)
            {
                list.Remove(listener);
            }

            cachedListeners.RemoveAll(c => c.Callback == listener);
        }

        public void Raise()
        {
            var tempListenersPriority = new SortedDictionary<int, List<Action>>(listenersByPriority);
            foreach (var pair in tempListenersPriority)
            {
                var tempListeners = new List<Action>(pair.Value);
                foreach (var listener in tempListeners)
                {
                    listener?.Invoke();
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