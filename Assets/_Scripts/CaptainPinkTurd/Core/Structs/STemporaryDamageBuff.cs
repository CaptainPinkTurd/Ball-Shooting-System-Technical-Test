namespace CaptainPinkTurd.Core.Struct
{
    public struct STemporaryDamageBuff
    {
        public int buffDamage;
        public int duration;
        public bool usageReducePerUses;
            
        public STemporaryDamageBuff(int buffDamage, int duration, bool usageReducePerUses)
        {
            this.buffDamage = buffDamage;
            this.duration = duration;
            this.usageReducePerUses = usageReducePerUses;
        }
    }
}