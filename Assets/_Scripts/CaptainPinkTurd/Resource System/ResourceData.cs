using UnityEngine;

namespace CaptainPinkTurd.ResourceSystem
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "Scriptable Objects/Resource")]
    public class ResourceData : ScriptableObject
    {
        public string id;
        public int maxAmount;
        [Tooltip("How many seconds does it take to recharge an unit of resource")]
        public int rechargeDurationPerUnit = 180;
    }
}
