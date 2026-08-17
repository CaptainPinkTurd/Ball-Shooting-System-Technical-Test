using UnityEngine;

namespace CaptainPinkTurd.ImprovedTimers
{
    /// <summary>
    /// Timer that counts down from a specific value to zero.
    /// </summary>
    public class CountdownTimer : Timer 
    {
        private readonly float eventTriggerTime;
        
        public override float Progress => 1f - Mathf.Clamp(CurrentTime / initialTime, 0, 1);
        
        /// <param name="value">The initial time for the timer</param>
        /// <param name="eventTriggerTime">When the mid-time event should be triggered.
        /// The value MUST not be greater nor smaller than the initial timer </param>
        /// <param name="ignoreTimeScale">Ignore timescale</param>
        public CountdownTimer(float value, float eventTriggerTime = 0, bool ignoreTimeScale = false) 
            : base(value, ignoreTimeScale)
        {
            //for example if value is 25 and triggerTime is 20, we will get the actual trigger time value of 0.2 due to this math
            this.eventTriggerTime = Mathf.Clamp(1 - eventTriggerTime / value, 0, 1);
        }

        public override void Tick() 
        {
            if (IsRunning && CurrentTime > 0) 
            {
                CurrentTime -= ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            if (Progress >= eventTriggerTime && !HasTriggeredEvent)
            {
                TriggerEventMidTimer();
            }
            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
    }
}