using System.Collections;
using UnityEngine;

namespace CaptainPinkTurd.SequenceSystem.Actions
{
    [CreateAssetMenu(menuName = "Sequencer Action/Wait", fileName = "New Wait Action")]
    public class SequencerActionWait : SequencerAction
    {
        [SerializeField] private float waitTime;
        
        public override IEnumerator StartSequence(Sequencer context)
        {
            yield return new WaitForSeconds(waitTime);
        }

        public override void Initialize(GameObject obj)
        {
            
        }
    }
}