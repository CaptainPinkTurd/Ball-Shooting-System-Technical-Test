using CaptainPinkTurd.Core.Interfaces;

namespace CaptainPinkTurd.ScoreSystem.Rule
{
    public class LowScoreRule : IScoreRule
    {
        public bool IsBetter(int newScore, int currentBest) => newScore < currentBest;
    }
}