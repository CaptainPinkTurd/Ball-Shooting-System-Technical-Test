using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.EffectSystem.KnockbackEffect;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.EffectSystem.ShakeEffect;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public abstract class UnitBase : GameObjectBase
    {
        [Header("Unit Base Config")] 
        [SerializeField][InlineScriptableObject] public UnitInfo unitInfo;
        [SerializeField] protected internal UnitHealth unitHealth;
        [SerializeField] private VoidEvent onKnockbackStart; 
        [SerializeField] private VoidEvent onKnockbackEnd; 
        
        [Header("Impact Configs")]
        [SerializeField] protected float hitStopDuration = 0.2f;
        [SerializeField] protected float deathHitStopDuration = 1f;
        [SerializeField] protected GameObjectShakeProfile gameObjectShakeProfile;

        public Knockback Knockback;
        
        public Rigidbody2D rb { get; private set; }
        public GameEvent OnDamageableKill { get; } = new GameEvent();
        
        protected override void Awake()
        {
            base.Awake();
            
            rb = GetComponent<Rigidbody2D>();
            Knockback = new Knockback(rb);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Knockback.OnKnockback.Subscribe(OnKnockbackEvents);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Knockback.OnKnockback.Unsubscribe(OnKnockbackEvents);
        }

        protected override void Start()
        {
            base.Start();
            
            unitHealth.OnDeath.Subscribe(OnDeath);
        }
        private void OnKnockbackEvents(bool isKnockback)
        {
            if (isKnockback)
            {
                onKnockbackStart.Raise();
            }
            else
            {
                onKnockbackEnd.Raise();
            }
        }
        public abstract void OnDamaged(SDamageData damageData);
        public abstract void OnDeath(SDamageData damageData);
    }
}