using CaptainPinkTurd.Core.Attributes;
using DG.Tweening;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    public class DefaultGameObjectShaker : MonoBehaviour
    {
        [SerializeField] private GameObjectShakeProfile shakeProfile;

        [Button("Shake")]
        public virtual void Shake()
        {
            if (!shakeProfile)
            {
                Debug.LogWarning("Shake profile is null, make sure to assign one in the inspector");
                OnShakeComplete();
                return;
            }
            
            transform.DOShakePosition(
                    shakeProfile.defaultShakeDuration,
                    shakeProfile.shakeStrength,
                    shakeProfile.vibration,
                    shakeProfile.randomness,
                    shakeProfile.snapping,
                    shakeProfile.fadeOut)
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(OnShakeComplete);
        }

        public virtual void ShakeWithProfile(GameObjectShakeProfile shakeProfile)
        {
            if (!shakeProfile)
            {
                Debug.LogWarning("Parameter shake profile is null");
                OnShakeComplete();
                return;
            }
            
            transform.DOShakePosition(
                    shakeProfile.defaultShakeDuration,
                    shakeProfile.shakeStrength,
                    shakeProfile.vibration,
                    shakeProfile.randomness,
                    shakeProfile.snapping,
                    shakeProfile.fadeOut)
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(OnShakeComplete);
        }

        protected virtual void OnShakeComplete() { }
    }
}