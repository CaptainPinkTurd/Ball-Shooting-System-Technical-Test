namespace CaptainPinkTurd.ImprovedTimers
{
    public sealed class FrequencyTimer : TickTimerBase
    {
        public int TickRate { get; private set; }
        public FrequencyTimer(float ticksPerSecond) 
        {
            CalculateTimeThreshold(ticksPerSecond);
        }

        protected override void CalculateTimeThreshold(float ticksPerSecond)
        {
            TickRate = (int)ticksPerSecond;
            timeThreshold = 1f / ticksPerSecond;
        }
    }
}