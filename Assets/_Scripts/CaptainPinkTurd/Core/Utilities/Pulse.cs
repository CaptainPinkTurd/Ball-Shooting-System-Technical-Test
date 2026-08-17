using DG.Tweening;
using UnityEngine;

namespace CaptainPinkTurd.Core.Utilities
{
    public class Pulse : MonoBehaviour
    {
        [Header("Pulse Settings")]
        [SerializeField] private float scaleMultiplier = 1.2f;
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private int loopCount = -1;

        [Header("Behaviour")]
        [SerializeField] private bool ignoreTimeScale;
        [SerializeField] private bool playOnEnable = true;
        
        private Vector3 originalScale;
        private Tween pulseTween;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        private void OnEnable()
        {
            if (playOnEnable) ActivatePulse();
        }

        private void OnDisable()
        {
            DeactivatePulse();
        }

        public void ActivatePulse()
        {
            DeactivatePulse();

            pulseTween = CreatePulseTween(loopCount);
        }

        public void ActivatePulseOneShot()
        {
            DeactivatePulse();

            pulseTween = CreatePulseTween(2);
        }

        private Tween CreatePulseTween(int loops)
        {
            Tween tween = transform
                .DOScale(originalScale * scaleMultiplier, duration)
                .SetEase(ease)
                .SetLoops(loops, LoopType.Yoyo)
                .SetUpdate(ignoreTimeScale);

            return tween;
        }

        public void DeactivatePulse()
        {
            if (pulseTween != null && pulseTween.IsActive())
            {
                pulseTween.Kill();
            }

            transform.localScale = originalScale;
        }
    }
}