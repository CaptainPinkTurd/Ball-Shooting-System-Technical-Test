using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Base;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ImpactEffect
{
    [CreateAssetMenu(fileName = "SpawnObjectEffect", menuName = "Scriptable Objects/Impact System/Spawn Object Effect")]
    public class SpawnObjectEffect : ScriptableObject
    {
        [Header("Spawn Object Configurations")]
        public GameObjectBase prefab;
        [Range(0f, 1f)] public float probability = 1;
        public bool isAttachedToImpactSurface;
        
        [Header("Rotation Configurations")]
        public bool canRotate;
        
        [ShowIf(nameof(canRotate))]
        public bool randomizeRotation;
        
        [ShowIf(nameof(canRotate))]
        [Tooltip("Zero values will lock the rotation on that axis. Values up to 360 are sensible for each X,Y,Z")]
        public Vector3 randomizedRotationMultiplier = Vector3.zero;
    }
}