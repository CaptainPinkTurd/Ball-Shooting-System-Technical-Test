using CaptainPinkTurd.Core.Attributes;
using Unity.Cinemachine;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    [CreateAssetMenu(fileName = "ScreenShakeProfile", menuName ="Scriptable Objects/Shake Profiles/New ScreenShake Profile")]
    public class ScreenShakeProfile : ScriptableObject
    {
        [Header("Impulse Source Settings")]
        public float impulseTime = 0.2f;
        public float impulseForce = 1f;
        public Vector3 defaultVelocity = new Vector3(0f, -1f, 0f);
        public CinemachineImpulseDefinition.ImpulseShapes impulseShape;
        [ShowIf("ImpulseShapeIsCustom")]
        public AnimationCurve impulseCustomShape;
    
        [Header("Impulse Listener Settings")]
        public float listenerAmplitude = 1f;
        public float listenerFrequency = 1f;
        public float listenerDuration = 1f;
        
        public bool ImpulseShapeIsCustom => impulseShape == CinemachineImpulseDefinition.ImpulseShapes.Custom;
    }
}