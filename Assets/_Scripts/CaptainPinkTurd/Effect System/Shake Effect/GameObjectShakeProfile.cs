using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    /// <summary>
    /// Applies a position shake using DOShakePosition with values from a shake profile.
    /// 
    /// If `useWithHitStop` is enabled, the shake duration will match the hit-stop duration;
    /// otherwise, it uses the profile's default shake duration.
    /// 
    /// Example:
    /// transform.DOShakePosition(
    ///     shakeProfile.useWithHitStop ? hitStopDuration : shakeProfile.defaultShakeDuration,
    ///     shakeProfile.shakeStrength,
    ///     shakeProfile.vibration,
    ///     shakeProfile.randomness,
    ///     shakeProfile.snapping,
    ///     shakeProfile.fadeOut)
    /// .SetUpdate(UpdateType.Normal, true);
    /// </summary>
    [CreateAssetMenu(fileName = "GameObjectShakeProfile", menuName ="Scriptable Objects/Shake Profiles/GameObject Shake Profile")]
    public class GameObjectShakeProfile : ScriptableObject 
    {
        public bool useWithHitStop;
        [ShowIf(nameof(useWithHitStop), false)] 
        public float defaultShakeDuration = 0.2f;
        [Tooltip("The shake strength on each axis")] 
        public Vector3 shakeStrength;
        [Tooltip("Indicates how much will the shake vibrate")]
        public int vibration = 10;
        [Tooltip("Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). Setting it to 0 will shake along a single direction.")]
        public float randomness = 90f;
        [Tooltip("If TRUE the tween will smoothly snap all values to integers")]
        public bool snapping = false;
        [Tooltip("If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not")]
        public bool fadeOut = true;
    }
}