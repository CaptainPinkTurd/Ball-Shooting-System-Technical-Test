using System;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.UnitSystem;
using DG.Tweening;
using UnityEngine;
namespace CaptainPinkTurd.Game.Player
{
    public class PlayerUnit : UnitBase
    {
        [Header("Player Unit Properties")]
        [SerializeField] private float delayOnDeath = 1f;
        
        [Header("Player Events")]
        [SerializeField] private VoidEvent onPlayerDamaged;
        [SerializeField] private VoidEvent onPlayerDeath;
        
        private void ApplyLayer(int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = layer;
            }
        }
        public override void OnDamaged(SDamageData damageData)
        {
            onPlayerDamaged.Raise();
        }

        public override void OnDeath(SDamageData damageData)
        {
            StopAllCoroutines();
            //onPlayerDeath.Raise();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            //if (!enemyLayers.Contains(other.gameObject.layer)) return;
            if (!other.TryGetComponentInHierarchy(out IDamageable enemyDamageable)) return;
            
            enemyDamageable.TakeDamage(new SDamageData(1, gameObject));
        }
    }

    [Serializable]
    public struct PlayerStateInfo
    {
        public GameObject model;
        public int layerValue;
    }
}