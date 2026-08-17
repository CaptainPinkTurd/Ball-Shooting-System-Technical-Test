using System.Collections.Generic;
using CaptainPinkTurd.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.UI.LayoutUI
{
    public abstract class LayoutMotionBase : MonoBehaviour
    {
        [Header("Layout Motion Base Properties")]
        [SerializeField] private string realTransformName;
        [SerializeField] private float motionDuration = .25f;

        [HideInInspector] public Vector3 spawnPosition;
        public RectTransform RealRectTransform { get; private set;}
        public LayoutElement RealRectTransformLayoutElement { get; private set;}
        
        public GameEvent onLayoutIndividualMotionStart = new GameEvent();
        public GameEvent onLayoutIndividualMotionEnd = new GameEvent();
        public GameEvent onLayoutGroupMotionEnd = new GameEvent();
        
        private RectTransform visualRectTransform;
        private Vector3 realRectTransformLastPos;
        private Queue<Vector3> moveQueue = new Queue<Vector3>();
        private bool tweenIsRunning;
        
        protected virtual void Awake()
        {
            visualRectTransform = GetComponent<RectTransform>();
        }

        protected virtual void OnEnable()
        {
            RealRectTransform?.gameObject.SetActive(true);
            Canvas.willRenderCanvases += CheckPosition;
        }

        private void CheckPosition()
        {
            if (RealRectTransform.position == realRectTransformLastPos) return;
            
            realRectTransformLastPos = RealRectTransform.position;
            MatchUIElement(visualRectTransform, RealRectTransform);
        }

        protected virtual void OnDisable()
        {
            RealRectTransform?.gameObject.SetActive(false);
            Canvas.willRenderCanvases -= CheckPosition;
        }

        public void GenerateEmptyRectTransform(Transform parent)
        {
            // Create a new empty GameObject
            GameObject childGameObject = new GameObject(realTransformName, typeof(RectTransform));
            
            childGameObject.GetComponent<RectTransform>().position = spawnPosition;
            childGameObject.transform.SetParent(parent);
            childGameObject.transform.localScale = Vector3.one;
            
            RealRectTransform = childGameObject.GetComponent<RectTransform>();
            RealRectTransform.sizeDelta = visualRectTransform.sizeDelta;
            RealRectTransformLayoutElement = childGameObject.AddComponent<LayoutElement>();
            
            realRectTransformLastPos = RealRectTransform.position;
            MatchUIElement(visualRectTransform, RealRectTransform, 0);
        }
        public void MatchUIElement(RectTransform source, RectTransform target, float? moveDuration = null)
        {
            Vector3 worldPos = target.position;
            
            // Convert world position into source's local space
            Vector3 localPos;
            if (source.parent is RectTransform parentRect)
            {
                localPos = parentRect.InverseTransformPoint(worldPos);
                
                moveQueue.Enqueue(localPos);

                if (!tweenIsRunning) DequeueAndPlay(source, moveDuration ?? motionDuration);
            }
            else
            {
                target.position = worldPos; // fallback
            }
        }
        private void DequeueAndPlay(RectTransform source, float moveDuration)
        {
            if (moveQueue.Count == 0)
            {
                tweenIsRunning = false;
                onLayoutGroupMotionEnd.Raise();
                return;
            }

            tweenIsRunning = true;
            var nextPos = moveQueue.Dequeue();
            
            onLayoutIndividualMotionStart.Raise();
            source.DOLocalMove(nextPos, moveDuration)
                .OnComplete(() =>
                {
                    // When finished, try to play the next
                    source.localPosition = nextPos;
                    DequeueAndPlay(source, moveDuration);
                    onLayoutIndividualMotionEnd.Raise();
                });
        }
    }
}
