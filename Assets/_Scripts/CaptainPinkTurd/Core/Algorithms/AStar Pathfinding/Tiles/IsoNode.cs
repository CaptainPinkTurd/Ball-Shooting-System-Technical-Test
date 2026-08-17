using System.Collections.Generic;
using CaptainPinkTurd.Core.Algorithm.AStar.Grid;
using UnityEngine;
using ZLinq;

namespace CaptainPinkTurd.Core.Algorithm.AStar.Tiles
{
    public class IsoNode : NodeBase 
    {
        private static readonly List<Vector2> Dirs = new List<Vector2>()
        {
            new Vector2(1, 0.5f), new Vector2(-1, 0.5f), new Vector2(1, -0.5f), new Vector2(-1, -0.5f)
        };

        public override void CacheNeighbors(GridManager gridManager)
        {
            Neighbors = new List<NodeBase>();

            foreach (var tile in Dirs.AsValueEnumerable().Select(dir => gridManager.GetNodeAtCoordPos(
                         Coords.Pos + dir)).Where(tile => tile != null)) {
                Neighbors.Add(tile);
            }
        }
    }
}