using System.Collections;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using UnityEngine;

namespace CaptainPinkTurd.Scene
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeInTime = .5f;
        [SerializeField] private float fadeOutTime = .5f;
        [SerializeField] private BoolEvent onFadeTransitionEnd;
        
        public IEnumerator FadeInBlack()
        {
            onFadeTransitionEnd.Raise(false);
            yield return FadeTo(1f, fadeInTime);
        }
        public IEnumerator FadeOutBlack()
        {
            yield return FadeTo(0f, fadeOutTime);
            onFadeTransitionEnd.Raise(true);
        }

        private IEnumerator FadeTo(float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
        }
    }
}