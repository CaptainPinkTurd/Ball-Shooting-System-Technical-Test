using CaptainPinkTurd.Core.Struct;
using UnityEngine;

namespace CaptainPinkTurd.Core.Interfaces
{
    public interface IDamageable 
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        bool IsDead => CurrentHealth <= 0;
        GameEvent<SDamageData> OnTakeDamage { get; }
        GameEvent<SDamageData> OnDeath { get; }
        void TakeDamage(SDamageData damageData);
        Transform GetTransform();
        IDamageable WithKnockback(Vector3 knockbackForce, float constantForce)
        {
            return this;
        }
        IDamageable WithSurfaceImpact(Vector3 hitPoint, Vector3 hitNormal)
        {
            return this;
        }
    }
}
