using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using ZLinq;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    public static class ShakeUtils
    {
        /// <summary>
        /// Applies synchronized shake animation to multiple transforms using the specified shake profile.
        /// All transforms will shake with the same random pattern for visual consistency.
        /// </summary>
        /// <param name="shakeObjects">List of transforms to shake</param>
        /// <param name="shakeProfile">Shake profile containing animation parameters</param>
        /// <param name="synchronizedRandomSeed">Random seed for synchronized shake pattern</param>
        public static void SynchronizedShakeWithProfile(List<Transform> shakeObjects, GameObjectShakeProfile shakeProfile, int synchronizedRandomSeed = 12345)
        {
            UnityEngine.Random.State originalState = UnityEngine.Random.state;
                
            float oldTimeScale = Time.timeScale;
            Time.timeScale = 0;

            foreach (var (shakeObj, index) in shakeObjects.AsValueEnumerable().Select((shakeObj, i) => (shakeObj, i)))
            {
                UnityEngine.Random.InitState(synchronizedRandomSeed);
                bool isLast = index == shakeObjects.Count - 1;
        
                shakeObj.DOShakePosition(shakeProfile.defaultShakeDuration,
                        shakeProfile.shakeStrength,
                        shakeProfile.vibration,
                        shakeProfile.randomness,
                        shakeProfile.snapping,
                        shakeProfile.fadeOut)
                    .SetUpdate(UpdateType.Normal, true)
                    .OnComplete(() =>
                    {
                        if (!isLast) return;
                        
                        Time.timeScale = oldTimeScale;
                    });
            }
            UnityEngine.Random.state = originalState;
        }
    }
}