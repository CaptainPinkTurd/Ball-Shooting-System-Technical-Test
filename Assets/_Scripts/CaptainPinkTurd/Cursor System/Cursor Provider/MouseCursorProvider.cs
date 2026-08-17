using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.CursorSystem.CursorProvider
{
    public class MouseCursorProvider : MonoBehaviour, ICursorProvider
    {
        public Vector3 Position { get; private set; }
        public bool IsActive => Application.isFocused && Time.timeScale > 0f;
        public GameEvent<Vector3> OnCursorPositionChange { get; } = new GameEvent<Vector3>();
        
        private void Update()
        {
            if(Position == MouseUtils.GetMouseWorldPosition()) return;
            
            Position = MouseUtils.GetMouseWorldPosition();
            OnCursorPositionChange.Raise(Position);
        }
    }
}