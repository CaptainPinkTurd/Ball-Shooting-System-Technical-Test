using CaptainPinkTurd.EffectSystem.KnockbackEffect;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public abstract class UnitInfo : ScriptableObject
    {
        [Header("Unit Info Config")]
        [SerializeField] internal string unitName;
        [SerializeField] internal KnockbackConfig defaultKnockbackConfig;
    }
}
