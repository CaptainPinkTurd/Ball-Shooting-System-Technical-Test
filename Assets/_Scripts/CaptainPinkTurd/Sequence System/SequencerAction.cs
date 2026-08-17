using System.Collections;
using UnityEngine;

namespace CaptainPinkTurd.SequenceSystem.Actions
{
    public abstract class SequencerAction : ScriptableObject
    {
        public abstract IEnumerator StartSequence(Sequencer context);
        public abstract void Initialize(GameObject obj);
    }
}