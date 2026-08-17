using UnityEngine;

namespace CaptainPinkTurd.Core
{
    [CreateAssetMenu(fileName = "TypewriterAudioInfo", menuName = "Scriptable Objects/TypewritterAudioInfo")]
    public class TypewriterAudioInfo : ScriptableObject
    {
        public string speaker;
        public AudioClip[] typingSoundClips;
        [Range(-3, 3)]
        public float minPitch = 1f;
        [Range(-3, 3)]
        public float maxPitch = 1f;
        public bool stopAudioSource;
        [Tooltip("Play sound every X characters")]
        public AnimationCurve soundFrequency = new AnimationCurve(
            new Keyframe(0f, 2f, -10f, -10f),
            new Keyframe(0.1f, 1f, 0f, 0f),
            new Keyframe(1f, 1f, -0f, 0f)
        );
    }
}
