using UnityEngine;

namespace CaptainPinkTurd.Core.CustomPhysics.Collision
{
    [CreateAssetMenu(fileName = "HeadBumpCollision2DCheckScriptableObject", 
        menuName = "Scriptable Objects/Physics/Collisions/Head Bump Collision2DCheckScriptableObject")]
    public class HeadBumpCollision2DCheckScriptableObject : Collision2DCheckBase
    {
        [Header("Head Collision Checks Configs")]
        public float headDetectionRayLength = 0.02f;
        [Range(0f, 1f)] public float headWidth = 0.75f;
        
        public bool BumpedHead(Collider2D feetColl, Collider2D bodyColl)
        {
            Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, bodyColl.bounds.max.y);
            Vector2 boxcastSize = new Vector2(feetColl.bounds.size.x * headWidth, headDetectionRayLength);
    
            var headHit = Physics2D.BoxCast(boxCastOrigin, boxcastSize, 
                0f, Vector2.down, headDetectionRayLength, groundLayer);
            var bumpedHead = headHit.collider;
    
            #region Debug Visualization

            if (!debugShowCollisionBoxes) return bumpedHead;
            
            var rayColor = bumpedHead ? Color.green : Color.red;
    
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x - boxcastSize.x / 2 * headWidth, boxCastOrigin.y),
                Vector2.up * headDetectionRayLength, 
                rayColor);
            
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x + boxcastSize.x / 2 * headWidth, boxCastOrigin.y), 
                Vector2.up * headDetectionRayLength, 
                rayColor);
            
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x - boxcastSize.x / 2 * headWidth, boxCastOrigin.y + headDetectionRayLength), 
                Vector2.right * boxcastSize.x * headWidth, 
                rayColor);
            #endregion
            
            return bumpedHead;
        }
    }
}