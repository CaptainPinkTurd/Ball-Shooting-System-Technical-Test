using System;
using CaptainPinkTurd.Core.Enum;
using UnityEngine;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class EModifierTypeExtensions
    {
        /// <summary>
        /// Applies the modifier to the given value using the provided amount.
        /// </summary>
        public static float Apply(this EModifierType modifierType, float value, float amount)
        {
            switch (modifierType)
            {
                case EModifierType.Additive:
                    // value + Amount
                    return value + amount;
    
                case EModifierType.Multiplicative:
                    // value * Amount
                    return value * amount;
    
                case EModifierType.AdditivePercent:
                    // value + (value * Amount)  (Amount = 0.2 => +20%)
                    return value + (value * amount);
    
                case EModifierType.MultiplicativePercent:
                    // value * (1 + Amount)  (Amount = 0.2 => *1.2)
                    return value * (1f + amount);
    
                case EModifierType.Override:
                    // value = Amount
                    return amount;
    
                case EModifierType.MinClamp:
                    // value = Mathf.Max(value, Amount)
                    return Mathf.Max(value, amount);
    
                case EModifierType.MaxClamp:
                    // value = Mathf.Min(value, Amount)
                    return Mathf.Min(value, amount);
    
                case EModifierType.Inverse:
                    // value = 1f / value
                    // (optionally guard against zero)
                    if (Mathf.Approximately(value, 0f))
                        return value; // or choose another fallback
                    return 1f / value;
    
                case EModifierType.Exponential:
                    // value = Mathf.Pow(value, Amount)
                    return Mathf.Pow(value, amount);
    
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifierType), modifierType, "Unknown modifier type");
            }
        }
    }
}