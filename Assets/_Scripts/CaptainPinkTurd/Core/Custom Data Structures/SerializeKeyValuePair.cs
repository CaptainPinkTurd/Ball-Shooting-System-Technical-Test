using System;

namespace CaptainPinkTurd.Core.CustomDataStructure
{
    [Serializable]
    public struct SerializeKeyValuePair<TKey, TValue>
    {
        public TValue Value;
        public TKey Key;
    }
}