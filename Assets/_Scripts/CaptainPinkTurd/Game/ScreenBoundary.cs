using System;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ScreenBoundary : MonoBehaviour
    {
        [Header("Solid Walls (bounce)")]
        [SerializeField] private BoxCollider2D wallTop;
        [SerializeField] private BoxCollider2D wallBottom;
        [SerializeField] private BoxCollider2D wallLeft;
        [SerializeField] private BoxCollider2D wallRight;
        [SerializeField] private float wallThickness = 0.5f;
        
        private Camera cam;
        private BoxCollider2D boxCollider;
        
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();

            if (!cam) cam = Camera.main;

            ResizeToCamera();
        }

        private void ResizeToCamera()
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;
            
            boxCollider.size = new Vector2(halfWidth * 2f, halfHeight * 2f);
            boxCollider.offset = Vector2.zero;
            
            PositionWall(wallTop,
                new Vector2(0f, halfHeight + wallThickness * 0.5f), //offset it to half the wall thickness to match with the exact border of the screen
                new Vector2(halfWidth * 2f + wallThickness * 2f, wallThickness)); //top and bottom wall need to add an additional thickness to complete the corner of the border

            PositionWall(wallBottom,
                new Vector2(0f, -halfHeight - wallThickness * 0.5f),
                new Vector2(halfWidth * 2f + wallThickness * 2f, wallThickness));

            PositionWall(wallLeft,
                new Vector2(-halfWidth - wallThickness * 0.5f, 0f),
                new Vector2(wallThickness, halfHeight * 2f));

            PositionWall(wallRight,
                new Vector2(halfWidth + wallThickness * 0.5f, 0f),
                new Vector2(wallThickness, halfHeight * 2f));
        }

        private void PositionWall(BoxCollider2D wall, Vector2 worldPosition, Vector2 size)
        {
            if (!wall)
            {
                Debug.LogWarning($"[{nameof(ScreenBoundary)}] haven't assigned a reference for wall yet, return early");
                return;
            }

            wall.transform.position = worldPosition;
            wall.size = size;
            wall.offset = Vector2.zero;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out BallBase ball)) return;

            //Debug.Log(ball.name + " out of bound, despawn immediately.");
            ball.Despawn();
        }
    }
}