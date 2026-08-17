using System.Collections;
using CaptainPinkTurd.AudioSystem;
using UnityEngine;

namespace CaptainPinkTurd.SequenceSystem.Actions
{
    [CreateAssetMenu(menuName = "Sequencer Action/Sound", fileName = "New Sound Action")]
    public class SequencerActionSound : SequencerAction
    {
        [SerializeField] private SoundData soundToPlay;
        [SerializeField] private bool randomPitch;
        
        private SoundBuilder soundBuilder;
        
        public override IEnumerator StartSequence(Sequencer context)
        {
            if (randomPitch)
            {
                soundBuilder.WithPosition(context.transform.position)
                    .WithRandomPitch().Play(soundToPlay);
            }
            else
            {
                soundBuilder.WithPosition(context.transform.position).Play(soundToPlay);
            }
            
            yield return null;
        }
        public override void Initialize(GameObject obj)
        {
            soundBuilder = SoundManager.Instance.CreateSoundBuilder();
        }
    }
}