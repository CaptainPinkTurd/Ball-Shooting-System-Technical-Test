using System.Collections;
using System.Collections.Generic;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.ImprovedTimers;
using CaptainPinkTurd.SpawnSystem.LootDropSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.SpawnSystem.Procedural
{
    public class ProceduralObjectSpawner : MonoBehaviour, ISpawner
    {
        [Header("Spawn Region Config")]
        [SerializeField] private float radius = 1;
        [SerializeField] private Vector2 regionSize = Vector2.one;
        [SerializeField] private int rejectionThreshold = 30;
        [SerializeField] private float displayRadius = 1;
        
        [Header("Spawn Config")]
        [SerializeField] private LootDropTableGameObjectProfile[] lootDropTableProfiles;
        [SerializeField, ReadOnly] private int chosenLootDropIndex;
        [SerializeField] private LayerMask blockingMask;
        [SerializeField] private float spawnDelay = .5f;
        [SerializeField] private bool spawnOnStart = true;
        [ShowIf(nameof(spawnOnStart))]
        [SerializeField] private int numberOfObjectsToSpawn = 3;
        [SerializeField] private bool assignSpawnedObjectAsChild = true;
        
        [Header("Respawn Config")]
        [SerializeField] private bool respawnOnInterval;
        [ShowIf(nameof(respawnOnInterval))]
        [SerializeField] private int spawnInterval = 300;
        
        public Vector2 RegionCenter => (Vector2)transform.position + regionSize * 0.5f;
        public List<GameObject> SpawnedObjects { get; private set; } = new List<GameObject>();
        public int CurrentActiveObjects => SpawnedObjects.Count;
        public GameEvent OnObjectDespawn { get; private set; } = new GameEvent();
        
        private bool spawnable = true;
        
        private List<Vector2> points;
        private IntervalTimer respawnTimer;

        private void Awake()
        {
            if (!respawnOnInterval) return;
            
            respawnTimer = new IntervalTimer(spawnInterval);
        }

        private void OnEnable()
        {
            if (!respawnOnInterval) return;
            
            respawnTimer.OnTick += RespawnEvent;
        }
        private void OnDisable()
        {
            if (!respawnOnInterval) return;
            
            respawnTimer.OnTick -= RespawnEvent;
        }

        private void Start()
        {
            respawnTimer?.Start();
            
            if (!spawnOnStart) return;

            spawnable = true;
            StartCoroutine(SpawnObjects(numberOfObjectsToSpawn));
        }

        public void SetSpawnableState(bool spawnable) => this.spawnable = spawnable;
        private void RespawnEvent()
        {
            //in case coroutine is still waiting to spawn objects from last interval
            if (spawnable) return;
            
            spawnable = true;
            StartCoroutine(CoroutineUtils.WaitForCondition(() => !Camera.main.IsRegionInView(RegionCenter, regionSize),
                () => StartCoroutine(SpawnObjects(numberOfObjectsToSpawn))));
        }

        public IEnumerator SpawnObjects(int amount)
        {
            if (!spawnable) yield return null;
            
            spawnable = false;
            
            chosenLootDropIndex = Random.Range(0, lootDropTableProfiles.Length);
            points = PoissonDiscSampling.GeneratePoints(radius, regionSize, 
                transform.position, rejectionThreshold);
            
            int spawnCount = Mathf.Min(amount, points.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                var selectedItem = lootDropTableProfiles[chosenLootDropIndex]
                    .lootDropTable.PickLootDropItem();
                int spawnIndex = i;
                
                while (!IsValidPlacement(points[spawnIndex], selectedItem.placementRadius))
                {
                    // Debug.LogWarning($"Point {points[spawnIndex]} is not a valid placement for item {selectedItem.item.name}" +
                    //                  ", continue to a different point instead");
                    spawnIndex++;

                    if (spawnIndex < points.Count) continue;
                    
                    //Debug.LogWarning($"No valid placement found for item {selectedItem.item.name}, skip spawning instead");
                    break;
                }
                
                if (spawnIndex >= points.Count) continue;

                GameObject spawnedObject;
                if (assignSpawnedObjectAsChild)
                {
                    spawnedObject = ObjectPoolManager.Instance.SpawnObject(selectedItem.item, transform);
                    spawnedObject.transform.position = points[spawnIndex];
                    spawnedObject.transform.rotation = selectedItem.spawnRandomRotation ?
                        Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)) : Quaternion.identity;
                }
                else
                {
                    spawnedObject = ObjectPoolManager.Instance.SpawnObject(selectedItem.item, points[spawnIndex],
                        selectedItem.spawnRandomRotation
                            ? Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))
                            : Quaternion.identity, ObjectPoolManager.PoolType.GameObject);
                }
                
                //assign auto remove from list if it's GameObjectBase
                if (spawnedObject.TryGetComponent<GameObjectBase>(out var goBase))
                {
                    goBase.SetSpawnedFromPool(true);
                    
                    goBase.OnDisableEvents.RefreshWithCachedListeners();
                    goBase.OnDisableEvents.Subscribe(() =>
                    {
                        SpawnedObjects.Remove(spawnedObject);
                        OnObjectDespawn.Raise();
                    }, EPriority.Medium, false);
                }
                
                SpawnedObjects.Add(spawnedObject);
                
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        public void DespawnObject(GameObject obj)
        {
            ObjectPoolManager.Instance.ReturnObjectToPool(obj);
            SpawnedObjects.Remove(obj);
            OnObjectDespawn.Raise();
        }

        public void DespawnAllObjects()
        {
            if(!gameObject.scene.isLoaded) return;
            
            var tempObjects = new List<GameObject>(SpawnedObjects);
            foreach (var spawnedObject in tempObjects)
            {
                ObjectPoolManager.Instance.ReturnObjectToPool(spawnedObject);
                OnObjectDespawn.Raise();
            }
            SpawnedObjects.Clear();
        }
        private bool IsValidPlacement(Vector2 pos, float placementRadius)
        {
            // Returns true if nothing overlaps the circle
            return !Physics2D.OverlapCircle(pos, placementRadius, blockingMask);
        }

        private void OnValidate()
        {
            points = PoissonDiscSampling.GeneratePoints(radius, regionSize, 
                transform.position, rejectionThreshold);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(RegionCenter, regionSize);
            
            if(points == null) return;

            foreach (Vector2 point in points)
            {
                Gizmos.DrawSphere(point, displayRadius);

                if (lootDropTableProfiles.Length <= 0 || !lootDropTableProfiles[0].itemDebugVisualize) continue;
                
                var lootDropItems = lootDropTableProfiles[0].lootDropTable.lootDropItems;
                foreach (var item in lootDropItems)
                {
                    Gizmos.color = item.gizmoColor;
                    Gizmos.DrawWireSphere(point, item.placementRadius);
                }
                Gizmos.color = Color.white;
            }
        }
    }
}