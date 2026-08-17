using System;
using UnityEngine;

namespace CaptainPinkTurd.ImprovedTimers
{
    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per second)
    /// </summary>
    public abstract class TickTimerBase : Timer 
    {
        public override float Progress
        {
            get
            {
                if (!IsRunning || timeThreshold <= 0f)
                    return 0f;

                return Mathf.Clamp01(CurrentTime / timeThreshold);
            }
        }
        public override bool IsFinished => !IsRunning;
        public Action OnTick = delegate { };
        
        protected float timeThreshold;

        protected TickTimerBase(bool ignoreTimeScale = false) : base(0, ignoreTimeScale) { }

        public override void Tick() 
        {
            if (IsRunning && CurrentTime >= timeThreshold) 
            {
                CurrentTime -= timeThreshold;
                OnTick.Invoke();
            }

            if (IsRunning && CurrentTime < timeThreshold)
            {
                CurrentTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }

        public override void Reset() 
        {
            CurrentTime = 0;
        }
        
        public void Reset(int newThresholdSeconds) 
        {
            CalculateTimeThreshold(newThresholdSeconds);
            Reset();
        }

        protected abstract void CalculateTimeThreshold(float thresholdSeconds);
    }
}