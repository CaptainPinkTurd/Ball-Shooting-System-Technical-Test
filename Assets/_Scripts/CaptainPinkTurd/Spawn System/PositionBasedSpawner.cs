using System.Collections.Generic;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem
{
    public class PositionBasedSpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private GameObjectBase spawnObjectPrefab;
        [SerializeField] private SoundData spawnSfx;
        
        public void SpawnFromAllPoints()
        {
            SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch().Play(spawnSfx);
            
            foreach (var spawnPoint in spawnPoints)
            {
                var spawnObj = ObjectPoolManager.Instance.SpawnObject(
                    spawnObjectPrefab.gameObject, spawnPoint.position, Quaternion.identity).GetComponent<GameObjectBase>();
                spawnObj.SetSpawnedFromPool(true);
            }
        }
    }
}