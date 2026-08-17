using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.ImprovedTimers;
using UnityEngine;

namespace CaptainPinkTurd.UI.TimerDisplayers
{
    public class StopwatchTimerDisplay : TimerDisplay
    {
        [Header("Stopwatch Timer Display Config")]
        [SerializeField] private bool hasTargetTime;
        
        [ShowIf(nameof(hasTargetTime))]
        [SerializeField] private float targetTime;
        
        // Cache to avoid unnecessary string updates
        private int lastHour;
        private int lastMinute;
        private int lastSecond;
        private int lastMillisecond;
        
        protected override void SetupTimer()
        {
            timer = new StopwatchTimer();
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