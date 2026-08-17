using System.Collections;
using CaptainPinkTurd.Input.Touch;
using UnityEngine;

namespace CaptainPinkTurd.Input.Swipe
{
    [RequireComponent(typeof(TouchInputReader2D))]
    public class SwipeDetection : MonoBehaviour
    {
        [SerializeField] private float minimumDistance = .2f;
        [SerializeField] private float maximumTime = 1f;
        [SerializeField][Range(0f, 1f)] private float directionThreshold = .9f;
        [SerializeField] private GameObject trail;
        
        private TouchInputReader2D swipeInputReader;
        private Coroutine trailCoroutine;
        
        private Vector2 startPosition;
        private float startTime;
        private Vector2 endPosition;
        private float endTime;

        private void Awake()
        {
            swipeInputReader = GetComponent<TouchInputReader2D>();
        }

        private void OnEnable()
        {
            swipeInputReader.OnStartTouch += SwipeStart;
            swipeInputReader.OnEndTouch += SwipeEnd;
        }

        private void OnDisable()
        {
            swipeInputReader.OnStartTouch -= SwipeStart;
            swipeInputReader.OnEndTouch -= SwipeEnd;
        }

        private void SwipeStart(Vector2 position, float time)
        {
            startPosition = position;
            startTime = time;
            
            trail.transform.position = position;
            trail.SetActive(true);
            
            trailCoroutine = StartCoroutine(Trail());
        }

        private void SwipeEnd(Vector2 position, float time)
        {
            trail.SetActive(false);
            StopCoroutine(trailCoroutine);
            
            endPosition = position;
            endTime = time;
            
            DetectSwipe();
        }

        private IEnumerator Trail()
        {
            while (true)
            {
                trail.transform.position = swipeInputReader.PrimaryPosition;
                yield return null;
            }
        }
        private void DetectSwipe()
        {
            if (Vector3.Distance(startPosition, endPosition) >= minimumDistance &&
                endTime - startTime <= maximumTime)
            {
                Debug.DrawLine(startPosition, endPosition, Color.red, 5f);
                
                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
            }
        }

        private void SwipeDirection(Vector2 direction)
        {
            if (Vector2.Dot(direction, Vector2.up) > directionThreshold)
            {
                Debug.Log("Swipe up");
            }
            else if (Vector2.Dot(direction, Vector2.down) > directionThreshold)
            {
                Debug.Log("Swipe down");
            }
            else if (Vector2.Dot(direction, Vector2.left) > directionThreshold)
            {
                Debug.Log("Swipe left");
            }
            else if (Vector2.Dot(direction, Vector2.right) > directionThreshold)
            {
                Debug.Log("Swipe right");
            }
        }
    }
}