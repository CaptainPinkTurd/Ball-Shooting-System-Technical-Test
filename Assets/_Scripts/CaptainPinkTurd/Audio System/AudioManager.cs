using System;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.DataPersistence;
using UnityEngine;
using UnityEngine.Audio;

namespace CaptainPinkTurd.AudioSystem
{
    public enum EVolumeType
    {
        Master = 0,
        Music = 1,
        SFX = 2
    }
    public class AudioManager : PersistentSingleton<AudioManager>, IDataPersistence
    {
        [Header("Mixer Groups References")]
        [SerializeField] private AudioMixerGroup masterMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        
        [Header("Mixer Parameters")]
        [SerializeField] private string masterVolumeParam = "masterVolume";
        [SerializeField] private string musicVolumeParam = "musicVolume";
        [SerializeField] private string sfxVolumeParam = "soundFxVolume";

        [Header("Data Config")] 
        [SerializeField] private bool saveSerializedVolumeDataOnStart;
        
        // Cached values (linear 0–1)
        [ShowIf(nameof(saveSerializedVolumeDataOnStart))] 
        [SerializeField] private AudioData audioData = new AudioData();
        
        public string Name => name;
        
        public AudioMixerGroup MasterMixerGroup => masterMixerGroup;
        public AudioMixerGroup MusicMixerGroup => musicMixerGroup;
        public AudioMixerGroup SfxMixerGroup => sfxMixerGroup;
        public float GetMasterVolume() => audioData.masterVolume;
        public float GetMusicVolume() => audioData.musicVolume;
        public float GetSFXVolume() => audioData.sfxVolume;

        private void Start()
        {
            if (saveSerializedVolumeDataOnStart)
            {
                Save();
                Load();
            }

            Debug.Log("Music Volume on start: " + audioData.musicVolume);
            ApplyAllSettings();
        }

        #region Public API

        public void SetVolume(EVolumeType volumeType, float value)
        {
            switch (volumeType)
            {
                case EVolumeType.Master:
                    audioData.masterVolume = value;
                    ApplyVolume(masterVolumeParam, value);
                    break;
                case EVolumeType.Music:
                    audioData.musicVolume = value;
                    ApplyVolume(musicVolumeParam, value);
                    break;
                case EVolumeType.SFX:
                    audioData.sfxVolume = value;
                    ApplyVolume(sfxVolumeParam, value);
                    break;
            }
            Save();
        }
        #endregion 
        
        #region Internal Logic
        private void ApplyAllSettings()
        {
            ApplyVolume(masterVolumeParam, audioData.masterVolume);
            ApplyVolume(musicVolumeParam, audioData.musicVolume);
            ApplyVolume(sfxVolumeParam, audioData.sfxVolume);
        }

        private void ApplyVolume(string parameter, float value)
        {
            // Convert linear (0–1) to logarithmic (dB)
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            masterMixerGroup.audioMixer.SetFloat(parameter, dB);
        }

        private void Save()
        {
            DataPersistenceManager.Instance.SaveGame();
        }

        private void Load()
        {
            DataPersistenceManager.Instance.LoadGame();
        }
        #endregion
        
        public void LoadData(object data)
        {
            //we don't want the initial load made by DataPersistenceManager to override our data
            if (!didStart && saveSerializedVolumeDataOnStart) return;

            audioData = (AudioData)data;
        }

        public object SaveData()
        {
            return audioData;
        }
        
        [Serializable]
        private class AudioData 
        {
            [Range(0f, 1f)] public float masterVolume = 1f;
            [Range(0f, 1f)] public float musicVolume = 1f;
            [Range(0f, 1f)] public float sfxVolume = 1f;
        }
    }
}