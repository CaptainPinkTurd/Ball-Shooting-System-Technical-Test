using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.CursorSystem.CursorProvider;
using UnityEngine;

namespace CaptainPinkTurd.CursorSystem.Visual
{
    [RequireComponent(typeof(ICursorProvider))]
    public class CursorVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cursorSprite;
        [SerializeField] private InterfaceReference<ICursorProvider> cursorProvider;

        private void OnEnable()
        {
            cursorProvider.Value.OnCursorPositionChange.Subscribe(OnCursorPositionChangeEvent);
        }
        private void OnDisable()
        {
            cursorProvider.Value.OnCursorPositionChange.Unsubscribe(OnCursorPositionChangeEvent);
        }

        private void Start()
        {
            Cursor.visible = false;
        }

        private void OnCursorPositionChangeEvent(Vector3 position)
        {
            if(!cursorProvider.Value.IsActive)
            {
                cursorSprite.enabled = false;
                Cursor.visible = true;
                return;
            }

            cursorSprite.enabled = true;
            transform.position = position;

            Cursor.visible = !Application.isFocused;
        }
    }
}