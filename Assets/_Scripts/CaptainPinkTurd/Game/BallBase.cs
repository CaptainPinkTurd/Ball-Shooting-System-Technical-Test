using System;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BallBase : GameObjectBase
    {
        [Header("Ball Base Properties")]
        [SerializeField] protected LayerMask collisionEventLayer;

        internal Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();
            
            rb = GetComponent<Rigidbody2D>();
        }

        protected abstract void OnBallCollisionEvent();

        protected void Despawn()
        {
            if (SpawnedFromPool)
            {
                ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (!collisionEventLayer.Contains(other.gameObject.layer)) return;
            
            OnBallCollisionEvent();
        }
    }
}