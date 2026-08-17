using System;
using UnityEngine;

namespace CaptainPinkTurd.AudioSystem
{
    public class SoundBuilder
    {
        private readonly SoundManager soundManager;
        private Vector3 position = Vector3.zero;
        
        private SoundEmitter currentSoundEmitter;  
        private bool randomPitch;
        private float minPitch;
        private float maxPitch;

        public SoundBuilder(SoundManager soundManager) 
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithPosition(Vector3 position) 
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch(float min = -0.05f, float max = 0.05f) 
        {
            minPitch = min;
            maxPitch = max;
            randomPitch = true;
            return this;
        }

        public void Play(SoundData soundData, Action onSoundEnd = null) 
        {
            if (soundData == null || !soundData.clip) 
            {
                Debug.LogWarning("SoundData is null");
                return;
            }
            
            if (soundManager && !soundManager.CanPlaySound(soundData)) return;

            currentSoundEmitter = soundManager.Get();
            
            if (!currentSoundEmitter)
            {
                Debug.LogError("No sound emitters available");
                return;
            }
            
            currentSoundEmitter.Initialize(soundData, 
                AudioManager.HasInstance ? AudioManager.Instance.SfxMixerGroup : null);
            currentSoundEmitter.transform.position = position;
            currentSoundEmitter.transform.parent = soundManager.transform;

            if (randomPitch) 
            {
                currentSoundEmitter.WithRandomPitch(minPitch, maxPitch);
            }

            if (soundData.frequentSound) 
            {
                currentSoundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(currentSoundEmitter);
            }
            
            currentSoundEmitter.onSoundEnd = onSoundEnd;
            currentSoundEmitter.onSoundEnd += () => currentSoundEmitter = null; //to make sure the sound emitter doesn't get mixed up with other builders once it's done playing sound
            currentSoundEmitter.Play();
        }

        public void StopCurrentSoundEmitter()
        {
            if (!currentSoundEmitter) return;
            
            currentSoundEmitter.Stop();
        }
    }
}
