using CaptainPinkTurd.Core.Extensions;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Utilities
{
    public class SpawnBoundaryUtils
    {
        //TODO: add a parameter that control what side to get a random point from as an option
        public static Vector2 GetRandomPointOutsideCamera(Camera cam, float buffer = 1f)
        {
            Bounds bounds = cam.GetOrthographicCameraBounds();

            float left   = bounds.min.x - buffer;
            float right  = bounds.max.x + buffer;
            float bottom = bounds.min.y - buffer;
            float top    = bounds.max.y + buffer;

            int side = Random.Range(0, 4);

            return side switch
            {
                // Left
                0 => new Vector2(
                    left,
                    Random.Range(bottom, top)
                ),

                // Right
                1 => new Vector2(
                    right,
                    Random.Range(bottom, top)
                ),

                // Bottom
                2 => new Vector2(
                    Random.Range(left, right),
                    bottom
                ),

                // Top
                _ => new Vector2(
                    Random.Range(left, right),
                    top
                ),
            };
        }
        public static bool TryGetValidPointOutsideCamera(Camera cam, float buffer, float checkRadius,
            LayerMask blockingMask, out Vector2 spawnPoint, int maxAttempts = 30)
        {
            Bounds bounds = cam.GetOrthographicCameraBounds();

            float left   = bounds.min.x - buffer;
            float right  = bounds.max.x + buffer;
            float bottom = bounds.min.y - buffer;
            float top    = bounds.max.y + buffer;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int side = Random.Range(0, 4);

                Vector2 candidate = side switch
                {
                    0 => new Vector2(left,  Random.Range(bottom, top)),    // Left
                    1 => new Vector2(right, Random.Range(bottom, top)),    // Right
                    2 => new Vector2(Random.Range(left, right), bottom),   // Bottom
                    _ => new Vector2(Random.Range(left, right), top),      // Top
                };

                if (Physics2D.OverlapCircle(candidate, checkRadius, blockingMask)) continue;
                
                spawnPoint = candidate;
                return true;
            }

            spawnPoint = Vector2.zero;
            return false;
        }
    }
}