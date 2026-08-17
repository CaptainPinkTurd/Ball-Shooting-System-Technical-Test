using System.Collections;
using CaptainPinkTurd.Core.Movement;
using UnityEngine;

namespace CaptainPinkTurd.Game.Interactions
{
    [RequireComponent(typeof(ColliderBoundaryClamp2D))]
    public class DraggableGameObject2D : InteractableGameObject2D
    {
        private Vector2 touchOffset;

        protected override void OnStartTouchEvent(Vector2 startPosition, float startTime)
        {
            if (!interactableZone.bounds.Contains(startPosition)) return;
            
            touchOffset = startPosition - (Vector2)transform.position;
            updateCoroutine = StartCoroutine(InteractionUpdate());
        }

        protected override IEnumerator InteractionUpdate()
        {
            while (true)
            {
                transform.position = touchInputReader2D.PrimaryPosition - touchOffset;
                yield return null;
            }
        }
    }
}