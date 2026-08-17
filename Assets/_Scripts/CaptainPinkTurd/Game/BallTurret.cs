using System;
using System.Collections;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.Game.Interactions;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class BallTurret : InteractableGameObject2D
    {
        [Header("Ball Turret Configs")] 
        [SerializeField] private Transform turretMuzzle;
        [Tooltip("Ball Per Seconds")]
        [SerializeField] private int fireRate = 2;
        [SerializeField] private float launchForce = 1f;
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private List<BallBase> ballPrefabShootOrder;
        
        private int currentBallIndex = 0;

        private void Update()
        {
            transform.LookAt2D(touchInputReader2D.PrimaryPosition, rotationSpeed);
        }

        protected override IEnumerator InteractionUpdate()
        {
            while (true)
            {
                SpawnBall();
                
                yield return new WaitForSeconds(1f / fireRate);
            }
        }
        
        private void SpawnBall()
        {
            var ballPrefab = ballPrefabShootOrder[currentBallIndex];
            var shootDirection = ((Vector3)touchInputReader2D.PrimaryPosition - turretMuzzle.position).normalized;
        
            BallBase ball = ObjectPoolManager.Instance.SpawnObject(ballPrefab.gameObject,
                turretMuzzle.transform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObject).GetComponent<BallBase>();
            ball.SetSpawnedFromPool(true);
            ball.rb.AddForce(shootDirection * launchForce, ForceMode2D.Impulse);
        
            currentBallIndex = (currentBallIndex + 1) % ballPrefabShootOrder.Count;
        }
    }
}