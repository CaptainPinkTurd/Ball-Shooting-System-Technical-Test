using System;
using System.Collections;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }
        public bool IsPlaying => audioSource.isPlaying;

        internal Action onSoundEnd;
        
        private AudioSource audioSource;
        private Coroutine playingCoroutine;

        private float minPitch;
        private float maxPitch;

        void Awake() {
            audioSource = gameObject.GetOrAdd<AudioSource>();
        }

        public void Initialize(SoundData data, AudioMixerGroup mixerGroup) 
        {
            Data = data;
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.overrideMixerGroup ? data.mixerGroup : mixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;
            
            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;
            
            audioSource.priority = data.priority;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;
            
            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = data.maxDistance;
            
            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;
            
            audioSource.rolloffMode = data.rolloffMode;
        }

        public void Play() 
        {
            if (playingCoroutine != null) 
            {
                StopCoroutine(playingCoroutine);
            }
            
            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        private IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            Stop();
        }

        public void Stop() 
        {
            onSoundEnd?.Invoke();
            onSoundEnd = null;
            
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }
            
            audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) 
        {
            minPitch = min;
            maxPitch = max;
            audioSource.pitch += Random.Range(minPitch, maxPitch);
        }
    }
}