using System.Collections.Generic;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ImpactEffect
{
    [CreateAssetMenu(fileName = "SurfaceEffect", menuName = "Scriptable Objects/Impact System/Surface Effect")]
    public class SurfaceEffect : ScriptableObject
    {
        [InlineScriptableObject] public List<SpawnObjectEffect> spawnObjectEffects = new List<SpawnObjectEffect>();
        public bool randomizedOneAudio;
        public List<SoundData> playAudioEffects = new List<SoundData>();
    }
}