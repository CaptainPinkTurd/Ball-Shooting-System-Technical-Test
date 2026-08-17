using System.Collections.Generic;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.Core.Utilities
{
    public class ParticleSystemController : GameObjectBase
    {
        private List<ParticleSystem> particles = new List<ParticleSystem>();

        protected override void Awake()
        {
            base.Awake();
            ParticleSystemSetup();
        }

        protected override void OnEnable()
        {
            OnEnableEvents.Subscribe(PlayParticles);
            
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            OnEnableEvents.Unsubscribe(PlayParticles);
        }
        private void ParticleSystemSetup()
        {
            if (TryGetComponent<ParticleSystem>(out var rootParticle))
            {
                particles.Add(rootParticle);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<ParticleSystem>(out var childParticle))
                {
                    particles.Add(childParticle);
                }
            }
        }

        private void PlayParticles()
        {
            int finishedParticles = 0;
            foreach (var particle in particles)
            {
                particle.Play();
                
                StartCoroutine(CoroutineUtils.WaitForSeconds(Time.deltaTime, () =>
                    StartCoroutine(CoroutineUtils.WaitForCondition(
                        () => !particle.IsAlive() || particle.particleCount == 0,
                        () =>
                        {
                            finishedParticles++;
                        }))));
            }
            
            StartCoroutine(CoroutineUtils.WaitForCondition(() => finishedParticles == particles.Count,
                () =>
                {
                    if(SpawnedFromPool)
                    {
                        ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
                    }
                    else
                    {
                        gameObject.SetActive(false);    
                    }
                }));
        }
    }
}