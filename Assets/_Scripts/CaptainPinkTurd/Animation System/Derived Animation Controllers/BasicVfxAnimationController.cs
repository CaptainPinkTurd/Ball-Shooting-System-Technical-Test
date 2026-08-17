using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public class BasicVfxAnimationController : AnimationControllerBase
    {
        [Header("Vfx Animation Controller Configs")]
        [SerializeField] private AnimationClip vfxAnimation;
        [SerializeField] private SoundData vfxSfx;

        public GameEvent OnAnimationEnd = new GameEvent();
        public override int DefaultAnimationHash { get; set; }
        
        protected override void OnEnable()
        {
            OnEnableEvents.Subscribe(PlayVfxAnimation);
            
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            OnEnableEvents.Unsubscribe(PlayVfxAnimation);
        }

        private void PlayVfxAnimation()
        {
            if (vfxSfx.clip)
            {
                SoundManager.Instance.CreateSoundBuilder()
                    .WithPosition(transform.position).WithRandomPitch().Play(vfxSfx);
            }
            
            PlayAnimation(Animator.StringToHash(vfxAnimation.name), onAnimationEnd: () =>
            {
                OnAnimationEnd.Raise();
                OnAnimationEnd.Clear();
                
                if(SpawnedFromPool)
                {
                    ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            });
        }
    }
}