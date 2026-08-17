using System.Collections.Generic;
using CaptainPinkTurd.Core.Algorithm.AStar.Tiles;
using UnityEngine;
using ZLinq;

namespace CaptainPinkTurd.Core.Algorithm.AStar
{
    /// <summary>
    /// This algorithm is written for readability. Although it would be perfectly fine in 80% of games, please
    /// don't use this in an RTS without first applying some optimization mentioned in the video: https://youtu.be/i0x5fj4PqP4
    /// If you enjoyed the explanation, be sure to subscribe!
    ///
    /// Also, setting colors and text on each hex affects performance, so removing that will also improve it marginally.
    /// </summary>
    public static class Pathfinding
    {
        private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);
        private static readonly Color OpenColor = new Color(.4f, .6f, .4f);
        private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f);
        
        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode) 
        {
            var toSearch = new List<NodeBase>() { startNode };
            var processed = new List<NodeBase>();
            var closestNode = startNode;
            var closestDistance = startNode.GetDistance(targetNode);

            while (toSearch.AsValueEnumerable().Any())
            {
                var current = toSearch[0];
                foreach (var t in toSearch)
                {
                    if (t.F < current.F || Mathf.Approximately(t.F, current.F) && t.H < current.H) current = t;
                }

                processed.Add(current);
                toSearch.Remove(current);

                current.SetColor(ClosedColor);

                if (current == targetNode)
                {
                    return GetPath(startNode, targetNode);
                }

                var distanceToTarget = current.GetDistance(targetNode);
                if (distanceToTarget < closestDistance)
                {
                    closestNode = current;
                    closestDistance = distanceToTarget;
                }

                foreach (var neighbor in current.Neighbors.AsValueEnumerable().Where(t =>
                             t.Walkable && (!t.Occupied || t == targetNode) && !processed.Contains(t)))
                {
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    if (inSearch && !(costToNeighbor < neighbor.G)) continue;
                    
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(current);

                    if (inSearch) continue;
                        
                    neighbor.SetH(neighbor.GetDistance(targetNode));
                    toSearch.Add(neighbor);
                    neighbor.SetColor(OpenColor);
                }
            }
            //if no path was found to targetNode (eg. it's blocked off by other units), path to the closest reachable tile instead
            return GetPath(startNode, closestNode);
        }

        private static List<NodeBase> GetPath(NodeBase startNode, NodeBase targetNode)
        {
            var currentPathTile = targetNode;
            var path = new List<NodeBase>();

            while (currentPathTile != startNode) 
            {
                path.Add(currentPathTile);
                currentPathTile = currentPathTile.Connection;
            }
                    
            foreach (var tile in path) tile.SetColor(PathColor);
            startNode.SetColor(PathColor);
                    
            //need to reverse path for intuitive use because it was adding in node from targetNode back to startNode
            path.Reverse();
            return path;
        }
    }
}