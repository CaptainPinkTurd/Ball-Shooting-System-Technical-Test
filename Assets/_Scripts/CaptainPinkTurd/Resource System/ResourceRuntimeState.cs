using System;

namespace CaptainPinkTurd.ResourceSystem
{
    [Serializable]
    public class ResourceRuntimeState
    {
        public int lastRegisteredAmount;
        public long checkpointTimeUtcTicks; // DateTime.UtcNow.Ticks
    }
}