using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.Core.CustomPhysics.Collision
{
    [CreateAssetMenu(fileName = "GroundCollision2DCheckScriptableObject",
        menuName = "Scriptable Objects/Physics/Collisions/Ground Collision2DCheckScriptableObject")]
    public class GroundCollision2DCheckScriptableObject : Collision2DCheckBase
    {
        [Header("Ground Collision Checks Configs")]
        [SerializeField] private float groundDetectionRayLength = 0.02f;
        [SerializeField] private bool useCustomBoxCastSize; 
        [ShowIf(nameof(useCustomBoxCastSize), true)]
        [SerializeField] private float customBoxCastSize;
        
        public bool IsGrounded(Collider2D feetColl)
        {
            Vector2 boxCastOrigin = new Vector2(feetColl.bounds.center.x, feetColl.bounds.min.y);
            Vector2 boxcastSize = new Vector2(useCustomBoxCastSize ? customBoxCastSize : feetColl.bounds.size.x,
                groundDetectionRayLength);
    
            var groundHit = Physics2D.BoxCast(boxCastOrigin, boxcastSize, 
                0f, Vector2.down, groundDetectionRayLength, groundLayer);
            
            var isGrounded = groundHit.collider;
    
            #region Debug Visualization

            if (!debugShowCollisionBoxes) return isGrounded;
            
            var rayColor = isGrounded ? Color.green : Color.red;
    
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x - (useCustomBoxCastSize ? customBoxCastSize / 2 : boxcastSize.x / 2), boxCastOrigin.y),
                Vector2.down * groundDetectionRayLength,
                rayColor
            );

            Debug.DrawRay(
                new Vector2(boxCastOrigin.x + (useCustomBoxCastSize ? customBoxCastSize / 2 : boxcastSize.x / 2), boxCastOrigin.y),
                Vector2.down * groundDetectionRayLength,
                rayColor
            );
            
            Debug.DrawRay(
                new Vector2(boxCastOrigin.x - (useCustomBoxCastSize ? customBoxCastSize / 2 : boxcastSize.x / 2), boxCastOrigin.y - groundDetectionRayLength), 
                Vector2.right * boxcastSize.x, 
                rayColor);

            #endregion
            
            return isGrounded;
        }
    }
}