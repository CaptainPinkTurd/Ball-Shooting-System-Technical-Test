using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using Unity.Cinemachine;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    public class CameraShakeManager : Singleton<CameraShakeManager>
    {
        [SerializeField] private bool useMultiSceneLevel;
        [ShowIf(nameof(useMultiSceneLevel), false)]
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [ShowIf(nameof(useMultiSceneLevel), false)]
        [SerializeField] private CinemachineImpulseListener impulseListener;
        
        [SerializeField] private bool ignoreTimeScale = true;
        [SerializeField] private float globalShakeForce = 1f;
        
        private CinemachineImpulseDefinition ImpulseDefinition => impulseSource.ImpulseDefinition;

        private void Start()
        {
            CinemachineImpulseManager.Instance.IgnoreTimeScale = ignoreTimeScale;
        }

        public void ScreenShake()
        {
            impulseSource.GenerateImpulseWithForce(globalShakeForce);
        }
        public void ScreenShakeFromProfile(ScreenShakeProfile profile)
        {
            SetupScreenShakeSettings(profile);
            impulseSource.GenerateImpulseWithForce(profile.impulseForce);
        }

        private void SetupScreenShakeSettings(ScreenShakeProfile profile)
        {
            ImpulseDefinition.ImpulseDuration = profile.impulseTime;
            impulseSource.DefaultVelocity = profile.defaultVelocity;
            if (profile.ImpulseShapeIsCustom)
            {
                ImpulseDefinition.CustomImpulseShape = profile.impulseCustomShape;
            }
            else
            {
                ImpulseDefinition.ImpulseShape = profile.impulseShape;
            }
            
            impulseListener.ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
            impulseListener.ReactionSettings.FrequencyGain = profile.listenerFrequency;
            impulseListener.ReactionSettings.Duration = profile.listenerDuration;
        }
        public void OnSceneLoaded()
        {
            if (!useMultiSceneLevel) return;
            
            //Camera.main.GetComponent<CinemachineBrain>().IgnoreTimeScale = ignoreTimeScale;
            impulseSource = FindAnyObjectByType<CinemachineImpulseSource>();
            impulseListener = FindAnyObjectByType<CinemachineImpulseListener>();
        }
    }
}