using CaptainPinkTurd.Core.Interfaces;

namespace CaptainPinkTurd.ScoreSystem.Rule
{
    public class HighScoreRule : IScoreRule
    {
        public bool IsBetter(int newScore, int currentBest) => newScore > currentBest;
    }
}