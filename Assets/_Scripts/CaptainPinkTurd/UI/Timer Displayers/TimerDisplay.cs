using System;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.ImprovedTimers;
using CaptainPinkTurd.UI.TextUI;
using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.UI.TimerDisplayers
{
    public abstract class TimerDisplay : MonoBehaviour
    {
        [Header("Timer Display Config")] 
        [SerializeField] private bool startTimerOnStart;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TextFormatRule timerFormatRule;

        [Header("Timer Display Events")] 
        [SerializeField] private VoidEvent onTimerStart;
        [SerializeField] private VoidEvent onTimerEnd;
        
        protected Timer timer;
        protected bool timerHasStarted;
        
        public float CurrentTime => timer.CurrentTime;

        private void Awake()
        {
            SetupTimer();
            
            if (!startTimerOnStart) return;
            
            StartTimer();
        }

        protected virtual void OnEnable()
        {
            timer.OnTimerStart += onTimerStart.Raise;
            timer.OnTimerStop += onTimerEnd.Raise;

            if (timer.IsRunning || !timerHasStarted) return;
            
            timer.Resume();
        }

        protected virtual void OnDisable()
        {
            timer.Pause();
            
            timer.OnTimerStart -= onTimerStart.Raise;
            timer.OnTimerStop -= onTimerEnd.Raise;
        }
        private void Update()
        {
            TimerUpdate();
        }

        protected void SetTimerTextByFormat(params object[] values)
        {
            timerText.text = timerFormatRule.Format(values);
        }
        public void StartTimer()
        {
            timerHasStarted = true;
            timer.Start();
        }
        protected void PauseTimer() => timer.Pause();
        protected abstract void SetupTimer();
        protected abstract void TimerUpdate();

        private void OnDestroy()
        {
            timer.Dispose();
        }
    }
}