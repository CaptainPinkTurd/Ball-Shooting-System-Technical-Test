using System.Collections;
using CaptainPinkTurd.Input.Touch;
using UnityEngine;

namespace CaptainPinkTurd.Game.Interactions
{
    public abstract class InteractableGameObject2D : MonoBehaviour
    {
        [Header("Interactable Game Object 2D Properties")]
        [SerializeField] protected Collider2D interactableZone;
        
        protected TouchInputReader2D touchInputReader2D;
        protected Coroutine updateCoroutine;
        
        protected virtual void Awake()
        {
            touchInputReader2D = FindAnyObjectByType<TouchInputReader2D>();
        }
        protected virtual void OnEnable()
        {
            touchInputReader2D.OnStartTouch += OnStartTouchEvent;
            touchInputReader2D.OnEndTouch += OnEndTouchEvent;
        }

        protected virtual void OnDisable()
        {
            touchInputReader2D.OnStartTouch -= OnStartTouchEvent;
            touchInputReader2D.OnEndTouch -= OnEndTouchEvent;
        }

        protected virtual void OnStartTouchEvent(Vector2 startPosition, float startTime)
        {
            if (!interactableZone.bounds.Contains(startPosition)) return;
            
            updateCoroutine = StartCoroutine(InteractionUpdate());
        }

        protected virtual void OnEndTouchEvent(Vector2 endPosition, float endTime)
        {
            if (updateCoroutine == null) return;
            
            StopCoroutine(updateCoroutine);
        }
        protected abstract IEnumerator InteractionUpdate();
    }
}