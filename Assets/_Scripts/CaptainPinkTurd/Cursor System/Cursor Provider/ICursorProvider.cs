using CaptainPinkTurd.Core;
using UnityEngine;

namespace CaptainPinkTurd.CursorSystem.CursorProvider
{
    public interface ICursorProvider
    {
        Vector3 Position { get; }
        bool IsActive { get; }
        GameEvent<Vector3> OnCursorPositionChange { get; }
    }
}
