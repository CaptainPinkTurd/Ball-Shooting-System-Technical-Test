using CaptainPinkTurd.Core.Base;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Wave
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Spawn System/Wave")]
    public class Wave : ScriptableObject
    {
        [SerializeField] internal GameObjectBase[] enemiesInWave;
        [SerializeField] internal int numberToSpawn;
    }
}