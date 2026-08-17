using UnityEngine;

namespace CaptainPinkTurd.ImprovedTimers 
{
    /// <summary>
    /// Timer that counts up from zero to infinity.  Great for measuring durations.
    /// </summary>
    public class StopwatchTimer : Timer
    {
        private readonly float? targetTime;
        
        public override float Progress => Mathf.Clamp(CurrentTime / targetTime.GetValueOrDefault(), 0, 1);
        public override bool IsFinished => CurrentTime >= targetTime; //false if targetTime is null
        
        public StopwatchTimer(float? targetTime = null, bool ignoreTimeScale = false) : base(0, ignoreTimeScale)
        {
            this.targetTime = targetTime;
        }

        public override void Tick() 
        {
            if (IsRunning) 
            {
                CurrentTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (IsFinished) 
            {
                Stop();
            }
        }
    }
}