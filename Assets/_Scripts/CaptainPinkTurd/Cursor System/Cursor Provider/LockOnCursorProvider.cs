using CaptainPinkTurd.Core;
using UnityEngine;

namespace CaptainPinkTurd.CursorSystem.CursorProvider
{
    //A generated class, haven't used it yet
    public class LockOnCursorProvider : MonoBehaviour, ICursorProvider
    {
        [SerializeField] private Transform target;

        public Vector3 Position => target.position;
        public bool IsActive => target;
        public GameEvent<Vector3> OnCursorPositionChange { get; } = new();

        private void LateUpdate()
        {
            if (target) OnCursorPositionChange.Raise(target.position);
        }
    }
}