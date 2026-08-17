using System.Collections;
using UnityEngine;

namespace CaptainPinkTurd.Core.Interfaces
{
    public interface ISpawner
    {
        int CurrentActiveObjects { get; }
        void SetSpawnableState(bool value);
        IEnumerator SpawnObjects(int amount);
        void DespawnObject(GameObject obj);
        void DespawnAllObjects();
    }
}
