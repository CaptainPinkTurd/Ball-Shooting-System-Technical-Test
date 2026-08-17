using UnityEngine;

namespace CaptainPinkTurd.Core.Movement
{
    public class ColliderBoundaryClamp2D : MonoBehaviour
    {
        [SerializeField] private Collider2D boundary;

        private void LateUpdate()
        {
            if (!boundary || boundary.bounds.Contains(transform.position)) return;
            
            Vector3 clampedPos = boundary.ClosestPoint(transform.position);
            transform.position = clampedPos;
        }
    }
}