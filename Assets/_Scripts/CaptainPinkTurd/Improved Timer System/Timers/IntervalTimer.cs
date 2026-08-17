namespace CaptainPinkTurd.ImprovedTimers
{
    public sealed class IntervalTimer : TickTimerBase
    {
        public float IntervalSeconds { get; private set; }
        public IntervalTimer(float intervalSeconds) 
        {
            CalculateTimeThreshold(intervalSeconds);
        }

        protected override void CalculateTimeThreshold(float intervalSeconds)
        {
            IntervalSeconds = intervalSeconds;
            timeThreshold = IntervalSeconds;
        }
    }
}