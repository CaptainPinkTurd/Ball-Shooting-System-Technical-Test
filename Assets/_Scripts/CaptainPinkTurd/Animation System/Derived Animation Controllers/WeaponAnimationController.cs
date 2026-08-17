using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public class WeaponAnimationController : AnimationControllerBase
    {
        [Header("Weapon Animation Controller Properties")]
        [SerializeField] private AnimationClip defaultAnimation;
        [SerializeField] private AnimationClip attackAnimation;
        
        public override int DefaultAnimationHash { get; set; }
        public bool IsAttacking { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            DefaultAnimationHash = Animator.StringToHash(defaultAnimation.name);
        }

        public void PlayAttackAnimation()
        {
            IsAttacking = true;
            PlayAnimation(Animator.StringToHash(attackAnimation.name));
        }
        public void SetAttackStateFalse() => IsAttacking = false;
    }
}