using System.Collections.Generic;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;

namespace CaptainPinkTurd.AudioSystem
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        [SerializeField] private List<AudioClip> initialPlaylist;
        
        private AudioSource current;
        private AudioSource previous;
        private const float crossFadeTime = 1.0f;
        private float fading;
        
        private readonly Queue<AudioClip> playlist = new();

        private void Start()
        {
            foreach (var clip in initialPlaylist) 
            {
                AddToPlaylist(clip);
            }
        }

        public void AddToPlaylist(AudioClip clip)
        {
            playlist.Enqueue(clip);
            if (!current && !previous) 
            {
                PlayNextTrack();
            }
        }

        public void Clear() => playlist.Clear();

        public void PlayNextTrack() 
        {
            if (playlist.TryDequeue(out AudioClip nextTrack)) 
            {
                Play(nextTrack);
            }
        }

        public void Play(AudioClip clip, bool loop = false) 
        {
            if (!clip) return;
            if (current && current.clip == clip && current.isPlaying) return;

            if (previous)
            {
                previous.Stop();
                previous = null;
            }

            previous = current;

            current = gameObject.GetOrAdd<AudioSource>();
            current.clip = clip;
            current.outputAudioMixerGroup = AudioManager.Instance.MusicMixerGroup; // Set mixer group
            current.loop = loop; // For playlist functionality, we want tracks to play once
            current.volume = 0;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }
        public void StopCurrentTrack() => current?.Stop();

        private void Update() 
        {
            HandleCrossFade();

            if (current && !current.isPlaying && playlist.Count > 0) 
            {
                PlayNextTrack(); 
            }
        }

        private void HandleCrossFade() 
        {
            if (fading <= 0f) return;
            
            fading += Time.unscaledDeltaTime;

            float fraction = Mathf.Clamp01(fading / crossFadeTime);

            // Logarithmic fade
            float logFraction = fraction.ToLogarithmicFraction();

            if (previous) previous.volume = 1.0f - logFraction;
            if (current) current.volume = logFraction;

            if (!(fraction >= 1)) return;
            
            fading = 0.0f;
            
            if (!previous || previous == current) return; // will need to come back to this shit later
            
            previous.Stop();
            previous = null;
        }
    }
}