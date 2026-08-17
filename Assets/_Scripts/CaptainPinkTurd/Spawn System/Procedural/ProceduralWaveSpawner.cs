// using System.Collections;
// using System.Collections.Generic;
// using CaptainPinkTurd.Core.Utilities;
// using CaptainPinkTurd.UnitSystem;
// using UnityEngine;
//
// namespace CaptainPinkTurd.SpawnSystem.Procedural
// {
//     public class ProceduralWaveSpawner : MonoBehaviour
//     {
//         [SerializeField] private List<ProceduralObjectSpawner> spawners;
//         [SerializeField] private int baseSpawn
//
//         private void Start()
//         {
//             WaveIncrement();
//         }
//
//         // ReSharper disable Unity.PerformanceAnalysis
//         private IEnumerator Spawn()
//         {
//             for (int i = 0; i < currentWave.numberToSpawn; i++)
//             {
//                 currentEnemiesInWaveCount++;
//                 int spawnPointIndex = Random.Range(0, spawnPoints.Count);
//                 int enemyIndex = Random.Range(0, currentWave.enemiesInWave.Length);
//
//                 UnitBase spawnUnit = ObjectPoolManager.Instance.SpawnObject(currentWave.enemiesInWave[enemyIndex].gameObject,
//                         spawnPoints[spawnPointIndex].position, Quaternion.identity, ObjectPoolManager.PoolType.Unit)
//                     .GetComponent<UnitBase>();
//                 spawnUnit.IsSpawnedFromPool = true;
//                 
//                 spawnUnit.OnDisableEvents.Clear();
//                 spawnUnit.OnDisableEvents.Subscribe(() =>
//                 {
//                     currentEnemiesInWaveCount--;
//                     WaveIncrement();
//                 });
//
//                 yield return new WaitForSeconds(spawnDelay);
//             }
//         }
//
//         private void WaveIncrement()
//         {
//             if (currentEnemiesInWaveCount > 0) return;
//             
//             currentEnemiesInWaveCount = 0;
//             
//             waveIndex = (waveIndex + 1) % waves.Length;
//             currentWave = waves[waveIndex];
//             
//             StartCoroutine(Spawn());
//         }
//     }
// }