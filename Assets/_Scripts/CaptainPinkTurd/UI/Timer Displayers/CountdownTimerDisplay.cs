using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.ImprovedTimers;
using UnityEngine;

namespace CaptainPinkTurd.UI.TimerDisplayers
{
    public class CountdownTimerDisplay : TimerDisplay
    {
        [Header("Countdown Timer Display Config")] 
        [SerializeField] private float startFromTime = 300f;

        [SerializeField] private bool useMidTimeEvent;
        [ShowIf(nameof(useMidTimeEvent))]
        [SerializeField] private float midTimeEvent = 60f;
        [ShowIf(nameof(useMidTimeEvent))]
        [SerializeField] private VoidEvent onMidTimerEvent;
        
        // Cache to avoid unnecessary string updates
        private int lastHour;
        private int lastMinute;
        private int lastSecond;
        private int lastMillisecond;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!useMidTimeEvent) return;
            timer.OnMidTimer += onMidTimerEvent.Raise;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (!useMidTimeEvent) return;
            timer.OnMidTimer -= onMidTimerEvent.Raise;
        }

        protected override void SetupTimer()
        {
            timer = new CountdownTimer(startFromTime, midTimeEvent);
        }

        protected override void TimerUpdate()
        {
            float time = timer.CurrentTime;
            
            int hours = Mathf.FloorToInt(time / 3600f);
            int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time - Mathf.Floor(time)) * 1000f);

            // Only update text if something actually changed
            if (hours == lastHour && minutes == lastMinute &&
                seconds == lastSecond && milliseconds == lastMillisecond) return;
            
            lastHour = hours;
            lastMinute = minutes;
            lastSecond = seconds;
            lastMillisecond = milliseconds;
            
            SetTimerTextByFormat(hours, minutes, seconds, milliseconds);
        }
    }
}