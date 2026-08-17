using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.Core.SO
{
    [CreateAssetMenu(fileName = "Damage Config", menuName = "Scriptable Objects/Weapons/Damage Config")]
    public class DamageConfiguration : ScriptableObject, ICloneable
    {
        public ParticleSystem.MinMaxCurve damageCurve;

        private void Reset()
        {
            damageCurve.mode = ParticleSystemCurveMode.Curve;
        }

        public int GetDamage(float distance = 0, float damageMultiplier = 1)
        {
            return Mathf.CeilToInt(
                damageCurve.Evaluate(distance, Random.value) * damageMultiplier);
        }

        public object Clone()
        {
            DamageConfiguration config = CreateInstance<DamageConfiguration>();
            
            config.damageCurve = damageCurve;
            return config;
        }
    }
}
