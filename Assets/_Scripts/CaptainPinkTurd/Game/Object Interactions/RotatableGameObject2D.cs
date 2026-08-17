using System.Collections;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;

namespace CaptainPinkTurd.Game.Interactions
{
    public class RotatableGameObject2D : InteractableGameObject2D
    {
        [Header("Rotation Configs")]
        [SerializeField] private float rotationSpeed = 360f;
        
        protected override IEnumerator InteractionUpdate()
        {
            while (true)
            {
                transform.LookAt2D(touchInputReader2D.PrimaryPosition, rotationSpeed);
                
                yield return null;
            }
        }
    }
}