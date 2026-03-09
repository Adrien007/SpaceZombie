//Enemy.cs

namespace SpaceZombie.Enemies
{
	public class Enemy
	{
		private int hp;
		public int Hp { get => hp; set => hp = value; }

		private int score;
		public int Score { get => score; private set => score = value; }

		public Enemy(int hp, int score)
		{
			this.hp = hp;
			this.score = score;
		}
	}

	public class EnemyFlagLogic
	{
		public bool isDead;
		public bool scoreGiven;
		public bool deadSoundPlayed;
		public bool endLevelAdded;

		public EnemyFlagLogic()
		{
			isDead = false;
			scoreGiven = false;
			deadSoundPlayed = false;
			endLevelAdded = false;
		}
	}
}
