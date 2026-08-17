using UnityEngine;

namespace CaptainPinkTurd.Core.Struct
{
    public struct SDamageData
    {
        public readonly int Amount;
        public readonly GameObject Source;  

        public SDamageData(int amount, GameObject source)
        {
            Amount = amount;
            Source = source;
        }
    }
}