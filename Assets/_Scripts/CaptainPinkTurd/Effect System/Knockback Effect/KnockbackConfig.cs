using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.KnockbackEffect
{
    [CreateAssetMenu(fileName = "KnockbackConfig", menuName = "Scriptable Objects/Knockback Config")]
    public class KnockbackConfig : ScriptableObject
    {
        public float knockbackTime = 0.2f;
        public Vector3 constantForceDirection = Vector3.up;
        public float inputForce = 7.5f;
        public AnimationCurve knockbackForceCurve;
    }
}
