using UnityEngine;

namespace CaptainPinkTurd.Core.CustomPhysics.Collision
{
    public abstract class Collision2DCheckBase : ScriptableObject
    {
        [Header("Collision Layers")]
        public LayerMask groundLayer;
        public LayerMask wallLayer;
        
        [Header("Debug Configs")]
        [SerializeField] protected bool debugShowCollisionBoxes;
    }
}