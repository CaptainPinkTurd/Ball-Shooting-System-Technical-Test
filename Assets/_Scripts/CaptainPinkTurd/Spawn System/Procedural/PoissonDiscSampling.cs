using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Procedural
{
    public static class PoissonDiscSampling
    {
        public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize,
            Vector2 sampleRegionOrigin, int numSamplesBeforeRejection = 30)
        {
            // ---------------------------------------------------------
            // 1. Normalize region bounds (supports negative sizes)
            //
            // We compute regionMin and regionMax so the sampler (regionSize) ALWAYS
            // works inside a POSITIVE coordinate system internally.
            //
            // Example:
            // Origin = (10,10), Size = (-50, 20)
            // Region actually goes from (-40, 10) to (10, 30)
            // ---------------------------------------------------------
            float minX = Mathf.Min(sampleRegionOrigin.x, sampleRegionOrigin.x + sampleRegionSize.x);
            float maxX = Mathf.Max(sampleRegionOrigin.x, sampleRegionOrigin.x + sampleRegionSize.x);
            float minY = Mathf.Min(sampleRegionOrigin.y, sampleRegionOrigin.y + sampleRegionSize.y);
            float maxY = Mathf.Max(sampleRegionOrigin.y, sampleRegionOrigin.y + sampleRegionSize.y);

            //regionMin is the local origin (0,0) of regionSize, it's the offset required to calculate world coordinates for the output 
            //(used to be just sampleRegionOrigin when negative size is not supported)
            Vector2 regionMin = new Vector2(minX, minY); 
            Vector2 regionMax = new Vector2(maxX, maxY);
            Vector2 regionSize = regionMax - regionMin; // always positive now

            // ---------------------------------------------------------
            // 2. Compute cell size for grid
            //
            // Entire Poisson algorithm uses LOCAL COORDINATES.
            // Grid is aligned to regionMin and extends to regionMax.
            // ---------------------------------------------------------
            float cellSize = radius / Mathf.Sqrt(2);

            int gridWidth  = Mathf.CeilToInt(regionSize.x / cellSize);
            int gridHeight = Mathf.CeilToInt(regionSize.y / cellSize);

            //store the index for each point currently stored in the points list based on the position the point got stored in the grid index
            int[,] grid = new int[gridWidth, gridHeight]; 

            List<Vector2> points = new();        // final WORLD points
            List<Vector2> spawnPoints = new();   // LOCAL points used for sampling
            
            // ---------------------------------------------------------
            // 3. Initial spawn point is placed in CENTER OF LOCAL SPACE.
            //
            // NOTE: spawnPoints are ALWAYS in LOCAL coordinates.
            // We convert to world space only when adding to output list.
            // ---------------------------------------------------------
            spawnPoints.Add(regionSize / 2);
            
            // ---------------------------------------------------------
            // 4. Poisson Disc Sampling Loop
            // ---------------------------------------------------------
            while (spawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Count);
                Vector2 spawnCenterLocal = spawnPoints[spawnIndex]; // still LOCAL space

                bool accepted = false;

                for (int i = 0; i < numSamplesBeforeRejection; i++)
                {
                    float angle = Random.value * Mathf.PI * 2;
                    Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    // candidate is generated in LOCAL space
                    Vector2 candidateLocal = spawnCenterLocal + dir * Random.Range(radius, 2 * radius);

                    // convert to WORLD for final output and distance checks
                    Vector2 candidateWorld = candidateLocal + regionMin;

                    if (IsValid(candidateLocal, candidateWorld, regionSize, cellSize, radius, points, grid))
                    {
                        // Store the WORLD point
                        points.Add(candidateWorld);

                        // Store LOCAL point into the spawn list
                        spawnPoints.Add(candidateLocal);

                        // Place into LOCAL grid
                        int cellX = (int)(candidateLocal.x / cellSize);
                        int cellY = (int)(candidateLocal.y / cellSize);
                        grid[cellX, cellY] = points.Count;

                        accepted = true;
                        break;
                    }
                }

                if (!accepted)
                    spawnPoints.RemoveAt(spawnIndex);
            }

            return points;
        }
        
        // ---------------------------------------------------------
        // VALIDITY CHECK
        //
        // - candidateLocal: used for grid lookup (LOCAL coordinates)
        // - candidateWorld: used for distance checking (WORLD coordinates)
        //
        // All grid indexing happens in LOCAL space,
        // but the final point and distances use WORLD space.
        // ---------------------------------------------------------
        private static bool IsValid( Vector2 candidateLocal, Vector2 candidateWorld, Vector2 regionSize,
            float cellSize, float radius, List<Vector2> points, int[,] grid)
        {
            // Local bounds check (works even if the original region was negative)
            if (candidateLocal.x < 0 || candidateLocal.x >= regionSize.x ||
                candidateLocal.y < 0 || candidateLocal.y >= regionSize.y)
                return false;

            // Convert to grid index (LOCAL)
            int cellX = (int)(candidateLocal.x / cellSize);
            int cellY = (int)(candidateLocal.y / cellSize);

            // Check neighbor cells
            int startX = Mathf.Max(cellX - 2, 0);
            int endX   = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int startY = Mathf.Max(cellY - 2, 0);
            int endY   = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex == -1) continue; //-1 means the point hasn't been occupied yet, therefore, skip

                    // Use WORLD coordinates for distance check
                    float sqrDist = (candidateWorld - points[pointIndex]).sqrMagnitude;
                    if (sqrDist < radius * radius)
                        return false;
                }
            }

            return true;
        }
    }
}