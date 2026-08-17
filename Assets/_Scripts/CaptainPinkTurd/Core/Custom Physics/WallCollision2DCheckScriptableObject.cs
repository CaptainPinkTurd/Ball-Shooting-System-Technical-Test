using UnityEngine;

namespace CaptainPinkTurd.Core.CustomPhysics.Collision
{
    [CreateAssetMenu(fileName = "WallCollision2DCheckScriptableObject", 
        menuName = "Scriptable Objects/Physics/Collisions/Wall Collision2DCheckScriptableObject")]
    public class WallCollision2DCheckScriptableObject : Collision2DCheckBase
    {
        [Header("Wall Collision Checks Configs")]
        public float wallDetectionRayLength = 0.125f;
        [Range(0.01f, 2f)] public float wallDetectionRayHeightMultiplier = 0.9f;
        
        public bool IsTouchingWall(Collider2D bodyColl, Transform transform, bool isFacingRight, ref RaycastHit2D lastWallHit)
        {
            float originEndPoint = 0f;
            originEndPoint = isFacingRight ? bodyColl.bounds.max.x : bodyColl.bounds.min.x;

            float adjustedHeight = bodyColl.bounds.size.y * wallDetectionRayHeightMultiplier;

            Vector2 boxCastOrigin = new Vector2(originEndPoint, bodyColl.bounds.center.y);
            Vector2 boxCastSize = new Vector2(wallDetectionRayLength, adjustedHeight);

            var wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, 
                wallDetectionRayLength, wallLayer);

            bool isTouchingWall = wallHit.collider;
            if (isTouchingWall) lastWallHit = wallHit;

            #region Debug Visualization

            if (!debugShowCollisionBoxes) return isTouchingWall;
            
            var rayColor = isTouchingWall ? Color.green : Color.red;

            Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
            Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

            Debug.DrawLine(boxBottomLeft, boxBottomRight, rayColor);
            Debug.DrawLine(boxBottomRight, boxTopRight, rayColor);
            Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
            Debug.DrawLine(boxTopLeft, boxBottomLeft, rayColor);

            #endregion
            
            return isTouchingWall;
        }
    }
}