using CaptainPinkTurd.Core.Struct;

namespace CaptainPinkTurd.Game.Enemy
{
    public class DummyUnit : EnemyUnitBase
    {
        public override void OnDeath(SDamageData damageData)
        {
            base.OnDeath(damageData);
            
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}