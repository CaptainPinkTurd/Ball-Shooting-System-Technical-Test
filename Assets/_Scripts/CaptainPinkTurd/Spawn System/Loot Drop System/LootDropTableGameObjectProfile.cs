using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.LootDropSystem
{
    [CreateAssetMenu(fileName = "LootDrop Table GameObject Profile", menuName = "Scriptable Objects/Loot Drop Table/LootDropTableGameObjectProfile")]
    public class LootDropTableGameObjectProfile : ScriptableObject
    {
        [Header("Loot Drop Table Config")] 
        [SerializeField] internal bool itemDebugVisualize;
        [SerializeField] internal LootDropTableGameObject lootDropTable;
        
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
        public class LootDropTableGameObject : GenericLootDropTable<LootDropItemGameObject, GameObject>
        {
            public void SetDebugVisualizeForItems(bool value)
            {
                foreach (var item in lootDropItems) item.SetDebugVisualize(value);
            }
        }

        [System.Serializable]
        public class LootDropItemGameObject : GenericLootDropItem<GameObject>
        {
            [Header("Loot Drop Item Spawn Config")]
            [Tooltip("Radius used for overlap & gizmo visualization for this item.")]
            public float placementRadius = 0.5f;
            public bool spawnRandomRotation = true;
            
            private bool debugVisualize;
            
            [Tooltip("Color to visualize this item’s radius in the scene view.")]
            [ShowIf(nameof(debugVisualize))]
            public Color gizmoColor = Color.green;
            
            public void SetDebugVisualize(bool value) => debugVisualize = value;
        }
    }
}