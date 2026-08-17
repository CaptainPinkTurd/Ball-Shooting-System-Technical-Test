using System;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.Input.Touch
{
    //Should be put under InputManager in the core scene
    public class TouchInputReader2D : MonoBehaviour
    {
        public event Action<Vector2, float> OnStartTouch;
        public event Action<Vector2, float> OnEndTouch;

        private void OnEnable()
        {
            StartCoroutine(CoroutineUtils.WaitForCondition(() => InputManager.HasInstance, () =>
            {
                InputManager.Instance.InputSystemActions.Touch.PrimaryContact.started += StartTouchPrimary;
                InputManager.Instance.InputSystemActions.Touch.PrimaryContact.canceled += EndTouchPrimary;
            }));
        }

        private void OnDisable()
        {
            if (!InputManager.HasInstance) return;
            
            InputManager.Instance.InputSystemActions.Touch.PrimaryContact.started -= StartTouchPrimary;
            InputManager.Instance.InputSystemActions.Touch.PrimaryContact.canceled -= EndTouchPrimary;
        }

        private void StartTouchPrimary(InputAction.CallbackContext ctx)
        {
            OnStartTouch?.Invoke(PrimaryPosition, (float)ctx.startTime);
        }
        private void EndTouchPrimary(InputAction.CallbackContext ctx)
        {
            OnEndTouch?.Invoke(PrimaryPosition, (float)ctx.time);
        }

        public Vector2 PrimaryPosition => Camera.main.ScreenToWorld2D(InputManager.Instance.InputSystemActions
            .Touch.PrimaryPosition.ReadValue<Vector2>());
    }
}