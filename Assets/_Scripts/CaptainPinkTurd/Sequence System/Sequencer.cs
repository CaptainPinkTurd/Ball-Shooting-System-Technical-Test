using System;
using System.Collections;
using System.Collections.Generic;
using CaptainPinkTurd.SequenceSystem.Actions;
using UnityEngine;

namespace CaptainPinkTurd.SequenceSystem
{
    public class Sequencer : MonoBehaviour
    {
        [SerializeField] private List<SequencerAction> sequenceActions;

        private void Awake()
        {
            foreach (var action in sequenceActions)
            {
                action.Initialize(gameObject);
            }
        }

        public void InitializeSequence()
        {
            StartCoroutine(ExecuteSequence());
        }

        private IEnumerator ExecuteSequence()
        {
            foreach (var action in sequenceActions)
            {
                yield return StartCoroutine(action.StartSequence(this));
            }
        }
    }
}