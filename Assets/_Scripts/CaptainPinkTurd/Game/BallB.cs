using System.Collections;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class BallB : BallBase
    {
        [Header("Ball B Configs")] 
        [SerializeField] private BallBase spawnBallPrefab;
        [SerializeField] private int splitBallAmount = 2;

        private Vector2 normalCollisionDirection;
        
        protected override void OnBallCollisionEvent()
        {
            StartCoroutine(SpawnBall());
        }

        private IEnumerator SpawnBall()
        {
            for (int i = 0; i < splitBallAmount; i++)
            {
                var spawnBall = ObjectPoolManager.Instance.SpawnObject(spawnBallPrefab.gameObject, 
                    transform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObject).GetComponent<BallBase>();
                spawnBall.SetSpawnedFromPool(true);
                spawnBall.rb.AddForce(normalCollisionDirection * 10, ForceMode2D.Impulse);
                
                yield return null;
            }
            Despawn();
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (!collisionEventLayer.Contains(other.gameObject.layer) || hasHitWall) return;
            
            normalCollisionDirection = other.GetContact(0).normal;
            base.OnCollisionEnter2D(other);
        }
    }
}