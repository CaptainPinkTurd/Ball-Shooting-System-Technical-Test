namespace CaptainPinkTurd.ScoreSystem
{
    public interface IScorable
    {
        ScoreConfig ScoreConfig { get; }
        void OnScored();
    }
}