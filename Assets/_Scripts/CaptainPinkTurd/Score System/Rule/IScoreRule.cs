namespace CaptainPinkTurd.Core.Interfaces
{
    public interface IScoreRule
    {
        bool IsBetter(int newScore, int currentBest);
    }
}