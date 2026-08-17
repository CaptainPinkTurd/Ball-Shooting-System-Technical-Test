using CaptainPinkTurd.DataPersistence;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Game/Level/Level Data")]
    public class LevelData : ScriptableObject
    {
        public bool hasDoneTutorial;
        public int totalLevelInGame = 6;

        public string Name => name;
    }
}
