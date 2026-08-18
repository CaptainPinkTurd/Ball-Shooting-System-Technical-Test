using UnityEngine;

namespace CaptainPinkTurd.Core.Utilities
{
    [RequireComponent(typeof(TrailRenderer))]
    public class ClampTrailLength : MonoBehaviour
    {
        [Tooltip("The maximum physical distance the trail can stretch.")]
        [SerializeField] private float maxLength = 5f; 
    
        private TrailRenderer trail;
        private Vector3 lastPosition;

        void Start()
        {
            trail = GetComponent<TrailRenderer>();
            lastPosition = transform.position;
        }

        void Update()
        {
            // Calculate current frame velocity magnitude
            float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
            lastPosition = transform.position;

            if (speed > 0.001f)
            {
                // Formula: Time = Distance / Speed
                trail.time = maxLength / speed;
            }
            else
            {
                // Set a tiny fallback time if the object stops completely
                trail.time = 0.01f;
            }
        }
    }
}