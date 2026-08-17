using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Interfaces;

namespace CaptainPinkTurd.ScoreSystem.Rule
{
    public static class ScoreRuleFactory
    {
        public static IScoreRule Create(EScoreRuleType type)
        {
            switch (type)
            {
                case EScoreRuleType.HigherIsBetter:
                    return new HighScoreRule();
    
                case EScoreRuleType.LowerIsBetter:
                    return new LowScoreRule();
    
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}