//ScoreManager.cs

namespace SpaceZombie.Scores.GameScore
{
    public interface IScoreManager
    {
        public int Score { get; set; }
    }
    public class ScoreManager : IScoreManager
    {
        private int score;

        public int Score
        {
            get => score;
            set
            {
                if (value >= 0)
                {
                    score += value;
                }
            }
        }

        public ScoreManager(int score)
        {
            Score = score;
        }
    }
}