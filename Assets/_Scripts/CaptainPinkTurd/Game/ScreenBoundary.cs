using System;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class ScreenBoundary : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out BallBase ball)) return;

            Debug.Log(ball.name + " out of bound, despawn immediately.");
            ball.Despawn();
        }
    }
}