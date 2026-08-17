using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.UnitSystem;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.LootDropSystem
{
    [CreateAssetMenu(fileName = "LootDrop Table UnitBase Profile", menuName = "Scriptable Objects/Loot Drop Table/LootDropTableUnitBaseProfile")]
    public class LootDropTableUnitBaseProfile : ScriptableObject
    {
        [Header("Loot Drop Table Config")] 
        [SerializeField] internal bool itemDebugVisualize;
        [SerializeField] internal LootDropTableUnitBase lootDropTable;
        
        private void OnEnable()
        {
            //Need to validate table to guarantee loot drop working correctly in builds cause OnValidate is only an editor thing
            lootDropTable?.ValidateTable();
            lootDropTable?.SetDebugVisualizeForItems(itemDebugVisualize);
        }
        
        public void OnValidate()
        {
            lootDropTable.ValidateTable();
            lootDropTable.SetDebugVisualizeForItems(itemDebugVisualize);
        }

        [System.Serializable]
        public class LootDropTableUnitBase : GenericLootDropTable<LootDropItemUnitBase, UnitBase>
        {
            public void SetDebugVisualizeForItems(bool value)
            {
                foreach (var item in lootDropItems) item.SetDebugVisualize(value);
            }
        }

        [System.Serializable]
        public class LootDropItemUnitBase : GenericLootDropItem<UnitBase>
        {
            [Header("Loot Drop Item Spawn Config")]
            [Tooltip("Radius used for overlap & gizmo visualization for this item.")]
            public float placementRadius = 0.5f;
            public LayerMask blockingMask;
            public bool spawnRandomRotation = true;

            private bool debugVisualize;
            
            [Tooltip("Color to visualize this item’s radius in the scene view.")]
            [ShowIf(nameof(debugVisualize))]
            public Color gizmoColor = Color.green;
            
            public void SetDebugVisualize(bool value) => debugVisualize = value;
        }
    }
}