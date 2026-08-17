using System.Collections.Generic;
using CaptainPinkTurd.Core.Algorithm.AStar.Tiles;
using UnityEngine;

namespace CaptainPinkTurd.Core.Algorithm.AStar.Grid.Scriptable 
{
    public abstract class ScriptableGrid : ScriptableObject 
    {
        [Header("Scriptable Grid Properties")]
        [SerializeField] protected NodeBase nodeBasePrefab;
        [SerializeField,Range(0,6)] private int _obstacleWeight = 3;
        public abstract Dictionary<Vector2, NodeBase> GenerateGrid();
        
        protected bool DecideIfObstacle() => Random.Range(1, 20) > _obstacleWeight;
    }
}
