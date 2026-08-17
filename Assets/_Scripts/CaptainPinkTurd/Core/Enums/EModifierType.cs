namespace CaptainPinkTurd.Core.Enum
{
    public enum EModifierType
    {
        Additive,          // value + Amount
        Multiplicative,    // value * Amount

        AdditivePercent,   // value + (value * Amount)  e.g. Amount = 0.2 => +20%
        MultiplicativePercent, // value * (1 + Amount) e.g. Amount = 0.2 => *1.2

        Override,          // value = Amount
        MinClamp,          // value = Mathf.Max(value, Amount)
        MaxClamp,          // value = Mathf.Min(value, Amount)

        Inverse,           // value = 1f / value (for certain stats)
        Exponential        // value = Mathf.Pow(value, Amount)
    }
}