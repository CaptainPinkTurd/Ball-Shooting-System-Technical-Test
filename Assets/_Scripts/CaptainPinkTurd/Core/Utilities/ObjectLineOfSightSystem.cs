using System.Collections;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace CaptainPinkTurd.Core.Utilities
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ObjectLineOfSightSystem : MonoBehaviour
    {
        [FormerlySerializedAs("layersToDetect")]
        [SerializeField] private LayerMask lineOfSightLayers;
        [SerializeField][Range(0f, 180f)] private float fieldOfView = 180;

        public GameEvent<GameObject> OnObjectDetect { get; } = new();
        public GameEvent<GameObject> OnObjectLoseSight { get; } = new();
        public bool IsDetected => isDetected;
        
        private CircleCollider2D circleCollider;
        private Coroutine checkForLineOfSightCoroutine;
        private bool isDetected;
        
        private void Awake()
        {
            circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
        }

        public void SetDetectionRadius(float radius)
        {
            StartCoroutine(CoroutineUtils.WaitForCondition(() => circleCollider,
                () => circleCollider.radius = radius));
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!lineOfSightLayers.Contains(other.gameObject.layer)) return;

            if (!CheckLineOfSight(other.gameObject) && checkForLineOfSightCoroutine == null)
            {
                checkForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.gameObject));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!lineOfSightLayers.Contains(other.gameObject.layer)) return;

            isDetected = false;
            OnObjectLoseSight.Raise(other.gameObject);
            
            if (checkForLineOfSightCoroutine == null) return;
            
            StopCoroutine(checkForLineOfSightCoroutine);
            checkForLineOfSightCoroutine = null;
        }

        private bool CheckLineOfSight(GameObject other)
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;

            Debug.Log("Is checking line of sight for " + other.name);
            if (Vector2.Dot(transform.right, direction) >= Mathf.Cos(fieldOfView * Mathf.Deg2Rad))
            {
                if (Physics2D.Raycast(transform.position, direction, Mathf.Infinity, lineOfSightLayers))
                {
                    isDetected = true;
                    OnObjectDetect.Raise(other);
                    return true;
                }
            }
            
            isDetected = false;
            return false;
        }

        private IEnumerator CheckForLineOfSight(GameObject other)
        {
            WaitForSeconds wait = new WaitForSeconds(0.1f);

            while (!CheckLineOfSight(other))
            {
                yield return wait;
            }
        }
    }
}