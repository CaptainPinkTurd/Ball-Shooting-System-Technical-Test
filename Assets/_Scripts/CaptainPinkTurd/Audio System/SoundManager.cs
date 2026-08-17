using System.Collections.Generic;
using UnityEngine;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using UnityEngine.Pool;

namespace CaptainPinkTurd.AudioSystem
{
    public class SoundManager : RegulatorSingleton<SoundManager>
    {
        [SerializeField] private SoundEmitter soundEmitterPrefab;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxSoundInstances = 30;
        
        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

        protected override void Awake()
        {
            base.Awake();
            
            InitializePool();
        }

        public SoundBuilder CreateSoundBuilder() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data) 
        {
            if (!data.frequentSound) return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances) 
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                    return true;
                } 
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get()
        {
            return soundEmitterPool?.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter) 
        {
            soundEmitterPool.Release(soundEmitter);
        }

        public void StopAll() 
        {
			LinkedList<SoundEmitter> tempList = new LinkedList<SoundEmitter>(activeSoundEmitters);

			foreach (var soundEmitter in tempList)
            {
                soundEmitter.Stop();
            }

            FrequentSoundEmitters.Clear();
        }
        public void StopSoundWithData(SoundData data)
        {
            foreach (var soundEmitter in activeSoundEmitters)
            {
                if (soundEmitter.Data != data) continue;
                
                soundEmitter.Stop();
                break;
            }
        }

        void InitializePool() 
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>
            (
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }

        SoundEmitter CreateSoundEmitter() 
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter) 
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(SoundEmitter soundEmitter) 
        {
            if (soundEmitter.Node != null) 
            {
                FrequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            if (!soundEmitter) return;
            
            Destroy(soundEmitter.gameObject);
        }
    }
}