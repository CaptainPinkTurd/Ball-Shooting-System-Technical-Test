using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.CursorSystem.CursorProvider;
using UnityEngine;

namespace CaptainPinkTurd.CursorSystem.Aim
{
    public class ObjectAimAtCursor : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<ICursorProvider> cursorProvider;

        private void OnEnable()
        {
            cursorProvider.Value.OnCursorPositionChange.Subscribe(OnCursorPositionChangeEvent);
        }

        private void OnDisable()
        {
            cursorProvider.Value.OnCursorPositionChange.Unsubscribe(OnCursorPositionChangeEvent);
        }

        private void OnCursorPositionChangeEvent(Vector3 position)
        {
            transform.LookAt2D(position, 10f);
        }
    }
}