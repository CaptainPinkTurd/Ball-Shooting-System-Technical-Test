using UnityEngine;

namespace CaptainPinkTurd.Core.Utils
{
    /// <summary>
    /// Utility class for common vector operations in 2D and 3D.
    /// </summary>
    public static class VectorUtils
    {
        /// <summary>
        /// Returns the direction vector from one position to another (normalized).
        /// </summary>
        public static Vector2 GetDirection(Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        /// <summary>
        /// Returns the angle (in degrees) from one position to another, useful for rotations.
        /// </summary>
        public static float GetAngle(Vector3 from, Vector3 to)
        {
            Vector2 dir = GetDirection(from, to);
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        
        /// <summary>
        /// Calculates a 2D velocity vector that moves from the current position toward the target position
        /// at a constant speed. 
        /// </summary>
        /// <param name="currentPosition">The object's current 2D position.</param>
        /// <param name="targetPosition">The desired 2D position to move toward.</param>
        /// <param name="followSpeed">The speed at which to move toward the target (units per second).</param>
        /// <returns>
        /// A velocity vector pointing from the current position toward the target, scaled by the follow speed.
        /// </returns>
        public static Vector2 GetVelocity2D(Vector2 currentPosition, Vector2 targetPosition, 
            float followSpeed)
        {
            Vector2 direction = (targetPosition - currentPosition).normalized;
            Vector2 velocity = direction * followSpeed;
        
            return velocity;
        }
    }
}
