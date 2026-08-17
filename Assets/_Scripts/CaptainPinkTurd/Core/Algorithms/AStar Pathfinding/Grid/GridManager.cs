using System.Collections.Generic;
using CaptainPinkTurd.Core.Algorithm.AStar.Grid.Scriptable;
using CaptainPinkTurd.Core.Algorithm.AStar.Tiles;
using CaptainPinkTurd.Core.Algorithm.AStar.Units;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using UnityEngine;

namespace CaptainPinkTurd.Core.Algorithm.AStar.Grid 
{
    public class GridManager : Singleton<GridManager> 
    {
        [Header("Grid Manager Properties")]
        [ShowIf(nameof(GridManagerOnlyField))]
        [SerializeField] private ScriptableGrid scriptableGrid;
        [SerializeField] private bool drawConnections;

        protected virtual bool GridManagerOnlyField => true;
        public Dictionary<Vector2, NodeBase> Tiles { get; protected set; }

        private NodeBase _playerNodeBase, _goalNodeBase;
        private Unit _spawnedPlayer, _spawnedGoal;
        
        private void Start() 
        {
            Tiles = scriptableGrid.GenerateGrid();
         
            foreach (var tile in Tiles.Values) tile.CacheNeighbors(this);
            
            NodeBase.OnHoverTile += OnTileHover;
        }

        private void OnDestroy() => NodeBase.OnHoverTile -= OnTileHover;

        private void OnTileHover(NodeBase nodeBase) 
        {
            _goalNodeBase = nodeBase;
            _spawnedGoal.transform.position = _goalNodeBase.Coords.Pos;

            foreach (var t in Tiles.Values) t.RevertTile();

            var path = Pathfinding.FindPath(_playerNodeBase, _goalNodeBase);
        }

        public NodeBase GetNodeAtCoordPos(Vector2 coordPos) => Tiles.GetValueOrDefault(coordPos);
        public virtual NodeBase GetNodeAtWorldPos(Vector3 pos) => Tiles.GetValueOrDefault(pos);
        public virtual Vector3 GetWorldPos(NodeBase node)
        {
            throw new System.NotImplementedException();
        }
        
        private void OnDrawGizmos() 
        {
            if (!Application.isPlaying || !drawConnections) return;
            
            Gizmos.color = Color.red;
            foreach (var tile in Tiles) 
            {
                if (!tile.Value.Connection) continue;
                Gizmos.DrawLine((Vector3)tile.Key + new Vector3(0, 0, -1), (Vector3)tile.Value.Connection.Coords.Pos + new Vector3(0, 0, -1));
            }
        }
    }
}