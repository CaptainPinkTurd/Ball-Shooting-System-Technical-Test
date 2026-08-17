using System;
using UnityEngine;

namespace CaptainPinkTurd.Core.Utilities
{
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField] private Transform targetPlayer;
        [SerializeField] private bool is2D;

        private void LateUpdate()
        {
            Vector3 newPos = targetPlayer.position;
            
            if(is2D)
            {
                newPos.z = -10;
            }
            else
            {
                newPos.y = transform.position.y;
            }
            
            transform.position = newPos;
        }
    }
}
