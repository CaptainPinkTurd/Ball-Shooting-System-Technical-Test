using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.LootDropSystem
{
    /// <summary>
    /// Item that can be picked by a LootDropTable.
    /// </summary>
    public abstract class GenericLootDropItem<T>
    {   
        [Header("Loot Drop Item Base Config")]
        [Tooltip("Item it represents - usually GameObject, integer etc...")]
        [SerializeField] internal T item;

        [Tooltip("How many units the item takes - more units, higher chance of being picked")]
        [SerializeField] internal float probabilityWeight;
        
        [SerializeField][ReadOnly] internal float probabilityPercent;

        // These values are assigned via LootDropTable script. They represent from which number to which number if selected, the item will be picked.
        internal float probabilityRangeFrom;
        internal float probabilityRangeTo;    
    }
}