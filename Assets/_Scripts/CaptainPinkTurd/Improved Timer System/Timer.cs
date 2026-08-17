using System;
using UnityEngine;

namespace CaptainPinkTurd.ImprovedTimers
{
    public abstract class Timer : IDisposable 
    {
        protected bool ignoreTimeScale = false;
        
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public abstract bool IsFinished { get; }
        public bool HasTriggeredEvent { get; private set; }
        
        [Tooltip("Will go from 0 (Start) to 1 (End) across all timer type")]
        public abstract float Progress { get; }

        protected float initialTime;

        public Action OnTimerStart = delegate { };
        public Action OnMidTimer = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value, bool ignoreTimeScale) 
        {
            initialTime = value;
            this.ignoreTimeScale = ignoreTimeScale;
        }

        public void Start() 
        {
            CurrentTime = initialTime;
            
            if (IsRunning) return;
            
            IsRunning = true;
            TimerManager.RegisterTimer(this);
            OnTimerStart?.Invoke();
        }

        public void TriggerEventMidTimer()
        {
            if (!IsRunning || HasTriggeredEvent) return;
            
            HasTriggeredEvent = true;
            OnMidTimer?.Invoke();
        } 
        protected void Stop()
        {
            if (!IsRunning) return;
            
            IsRunning = false;
            TimerManager.DeregisterTimer(this);
            OnTimerStop?.Invoke();
        }
        public void SetIgnoreTimeScale(bool ignore) => ignoreTimeScale = ignore;
        public abstract void Tick();

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }

        bool disposed;

        ~Timer() 
        {
            Dispose(false);
        }

        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // when the consumer is done with the timer or being destroyed
        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        {
            if (disposed) return;

            if (disposing)
            {
                TimerManager.DeregisterTimer(this);
            }

            disposed = true;
        }
    }
}