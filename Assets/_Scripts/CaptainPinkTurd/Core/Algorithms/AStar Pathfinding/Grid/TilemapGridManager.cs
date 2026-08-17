using System.Collections.Generic;
using CaptainPinkTurd.Core.Algorithm.AStar.Tiles;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaptainPinkTurd.Core.Algorithm.AStar.Grid
{
    public class TilemapGridManager : GridManager
    {
        [Header("Tilemap Grid Manager Properties")]
        [SerializeField] private Tilemap groundTilemap;     
        [SerializeField] private Tilemap collisionTilemap;  
        [SerializeField] private NodeBase nodePrefab;

        protected override bool GridManagerOnlyField => false;
        protected override void Awake()
        {
            base.Awake();
            
            Tiles = BuildNodesFromTilemap();
            foreach (var tile in Tiles.Values) tile.CacheNeighbors(this);
        }

        private void Start() { } //to override the GridManager start for now
        private Dictionary<Vector2, NodeBase> BuildNodesFromTilemap()
        {
            var tiles = new Dictionary<Vector2, NodeBase>();
            var bounds = groundTilemap.cellBounds;
            
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var cellPos = new Vector3Int(x, y, 0);
                    if (!groundTilemap.HasTile(cellPos)) continue;
                    
                    bool walkable = !collisionTilemap || !collisionTilemap.HasTile(cellPos);
                    var node = ObjectPoolManager.Instance.SpawnObject(nodePrefab.gameObject, 
                        groundTilemap.transform).GetComponent<NodeBase>();
                    // Use cell coordinates as Pos so CacheNeighbors offsets work correctly
                    if (node is SquareNode squareNode)
                    {
                        squareNode.Init(walkable, new SquareCoords { Pos = new Vector2(x, y) });
                        node.transform.localPosition = groundTilemap.GetCellCenterLocal(cellPos);
                    }
                    else
                    {
                        Debug.LogError("Node type not supported yet");
                    }
                    tiles[new Vector2(x, y)] = node;
                }
            }
            return tiles;
        }
        
        // Convert a world position to the node at that cell
        public override NodeBase GetNodeAtWorldPos(Vector3 worldPos)
        {
            var cell = groundTilemap.WorldToCell(worldPos);
            return Tiles.GetValueOrDefault(new Vector2(cell.x, cell.y)); //enemy tile is null ?
        }
        
        // Convert a node back to its world position (for moving units)
        public override Vector3 GetWorldPos(NodeBase node)
        {
            return groundTilemap.GetCellCenterWorld(
                new Vector3Int((int)node.Coords.Pos.x, (int)node.Coords.Pos.y, 0));
        }
    }
}