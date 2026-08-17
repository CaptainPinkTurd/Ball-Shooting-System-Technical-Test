using System.Collections.Generic;
using CaptainPinkTurd.Core.Algorithm.AStar.Grid;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;
using ZLinq;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.Core.Algorithm.AStar.Tiles
{
    public class SquareNode : NodeBase
    {
        [Header("Square Node Properties")]
        [SerializeField] private EDirectionMode directionMode;
        
        // private static readonly List<Vector2> EightDirs = new List<Vector2>() 
        // {
        //     new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0),
        //     new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1)
        // };

        public override void CacheNeighbors(GridManager gridManager)
        {
            Neighbors = new List<NodeBase>();
            var dirs = directionMode.GetVectorDirections();

            foreach (var tile in dirs.AsValueEnumerable().Select(dir => gridManager.GetNodeAtCoordPos(
                         Coords.Pos + dir)).Where(tile => tile != null))
            {
                Neighbors.Add(tile);
            }
        }

        public override void Init(bool walkable, ICoords coords)
        {
            base.Init(walkable, coords);

            _renderer.transform.rotation = Quaternion.Euler(0, 0, 90 * Random.Range(0, 4));
        }
    }

    public struct SquareCoords : ICoords
    {
        public float GetDistance(ICoords other)
        {
            var dist = new Vector2Int(Mathf.Abs((int)Pos.x - (int)other.Pos.x),
                Mathf.Abs((int)Pos.y - (int)other.Pos.y));

            var lowest = Mathf.Min(dist.x, dist.y);
            var highest = Mathf.Max(dist.x, dist.y);

            var horizontalMovesRequired = highest - lowest;

            return lowest * 14 + horizontalMovesRequired * 10 ;
        }
        public Vector2 Pos { get; set; }
    }
}
