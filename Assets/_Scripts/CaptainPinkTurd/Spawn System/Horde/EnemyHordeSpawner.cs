using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.ImprovedTimers;
using CaptainPinkTurd.SpawnSystem.Utilities;
using CaptainPinkTurd.UnitSystem;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Horde
{
    public class EnemyHordeSpawner : MonoBehaviour
    {
        [SerializeField] private HordeController hordeController;
        
        private HordePhase currentPhase;
        private readonly StopwatchTimer spawnTimer = new StopwatchTimer();
        private int currentEnemiesCount;

        private void Awake()
        {
            spawnTimer.Start();
        }

        private void OnEnable()
        {
            hordeController.onPhaseChange.Subscribe(SetPhase);
        }

        private void OnDisable()
        {
            hordeController.onPhaseChange.Unsubscribe(SetPhase);
        }

        private void SetPhase(HordePhase newPhase)
        {
            currentPhase = newPhase;
            spawnTimer.Reset();
        }
        
        void Update()
        {
            if (!currentPhase ||
                !(spawnTimer.CurrentTime >= currentPhase.spawnInterval)) return;

            spawnTimer.Reset();
            TryToSpawnEnemy();
        }

        private void TryToSpawnEnemy()
        {
            if (currentEnemiesCount >= currentPhase.maxActiveEnemies) return;
            
            currentEnemiesCount++;
            
            var lootDropItem = currentPhase.enemiesSpawnProfile.lootDropTable.PickLootDropItem();
            
            if(SpawnBoundaryUtils.TryGetValidPointOutsideCamera(Camera.main, 1, lootDropItem.placementRadius,
                lootDropItem.blockingMask, out Vector2 spawnPos))
            {
                UnitBase enemy = ObjectPoolManager.Instance.SpawnObject(lootDropItem.item.gameObject, spawnPos,
                    Quaternion.identity, ObjectPoolManager.PoolType.Unit).GetComponent<UnitBase>();
                
                enemy.OnDisableEvents.RefreshWithCachedListeners();
                enemy.OnDisableEvents.Subscribe(() =>
                {
                    currentEnemiesCount--;
                }, EPriority.Medium, false);
            }
            else
            {
                currentEnemiesCount--;
                Debug.LogError($"Couldn't find a valid point to spawn {lootDropItem.item.name} outside of camera");
            }
        }
    }
}