using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.SpawnSystem.LootDropSystem;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Horde
{
    [CreateAssetMenu(fileName = "Horde Phase", menuName = "Scriptable Objects/Spawn System/HordePhase")]
    public class HordePhase : ScriptableObject
    {
        [Header("Horde Phase Config")]
        [SerializeField] internal int startTime = 0;
        [SerializeField] internal int endTime = 60;
        
        [Header("Horde Enemies Config")]
        [SerializeField] internal int maxActiveEnemies = 10;
        [SerializeField] internal float spawnInterval = 1f;
        [SerializeField][InlineScriptableObject] internal LootDropTableUnitBaseProfile enemiesSpawnProfile;

        private void OnValidate()
        {
            if (startTime > endTime)
            {
                endTime = startTime;
            }
            enemiesSpawnProfile.OnValidate();
        }
    }
}