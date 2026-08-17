using System.Collections;
using UnityEngine;

namespace CaptainPinkTurd.Core.Interfaces
{
    public interface ISlowable
    {
        MonoBehaviour ActiveMonoBehaviour { get; }
        float BaseSpeed { get; }
        Coroutine SlowCoroutine { get; set; }

        void Slow(AnimationCurve slowDecay)
        {
            if(SlowCoroutine != null) ActiveMonoBehaviour.StopCoroutine(SlowCoroutine);
            if(!ActiveMonoBehaviour.isActiveAndEnabled) return;
            
            SlowCoroutine = ActiveMonoBehaviour.StartCoroutine(SlowDown(slowDecay));
        }
        protected IEnumerator SlowDown(AnimationCurve slowDecay);
    }
}