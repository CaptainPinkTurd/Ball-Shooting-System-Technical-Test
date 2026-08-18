using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class BallA : BallBase
    {
        [Header("Ball A Configs")] 
        [SerializeField] private float lifeTime = 2f;
        [SerializeField] private float explosionRadius = 2f;
        [SerializeField] private float explosionForce = 50f;
        [SerializeField] private LayerMask affectedByExplosionLayers;
        [SerializeField] private ParticleSystemController explosionVfxPrefab;
        [SerializeField] private SoundData explosionSfx;
        
        protected override void OnBallCollisionEvent()
        {
            StartCoroutine(CoroutineUtils.WaitForSeconds(lifeTime, Explode));
        }

        private void Explode()
        {
            Vector2 explosionPosition = transform.position;
            PlayExplosionEffects(explosionPosition);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                explosionPosition,
                explosionRadius,
                affectedByExplosionLayers.value
            );

            foreach (var collider in colliders)
            {
                Rigidbody2D rb = collider.attachedRigidbody;

                if (!rb) continue;

                Vector2 direction = rb.position - explosionPosition;

                if (direction.sqrMagnitude < 0.001f) continue;

                rb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);
            }
            
            Despawn();
        }

        private void PlayExplosionEffects(Vector3 position)
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(position).WithRandomPitch().Play(explosionSfx);
            
            var explosionVfx = ObjectPoolManager.Instance.SpawnObject(explosionVfxPrefab.gameObject, 
                position, Quaternion.identity, ObjectPoolManager.PoolType.VFX).GetComponent<ParticleSystemController>();
            explosionVfx.SetSpawnedFromPool(true);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}