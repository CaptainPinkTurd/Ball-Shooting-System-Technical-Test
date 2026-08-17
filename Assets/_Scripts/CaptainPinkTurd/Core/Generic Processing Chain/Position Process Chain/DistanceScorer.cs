namespace CaptainPinkTurd.Core.ProcessingChain
{
    public class DistanceScorer : IProcessor<float, float>
    {
        public float Process(float distance)
        {
            return 1f / (1f + distance);
        }
    }
}