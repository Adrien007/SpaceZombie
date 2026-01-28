//ScoreSystem.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using SpaceZombie.Scores.GameScore;

namespace SpaceZombie.Events
{
    public class ScoreSystem : IBulletCollisionOberser
    {
        private IScoreManager sm;

        public ScoreSystem(IScoreManager scoreManager)
        {
            sm = scoreManager;
        }

        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
                EnemyObjet enemy = EnemyEventHandler.TrouverParent(aera2D);
                if (EnemyEventHandler.IsEnemyDie(enemy) && !EnemyEventHandler.IsScoreGiven(enemy))
                {
                    enemy.enemyFlagLogic.scoreGiven = true; // Set the score given flag to true
                    GD.Print("[ScoreSystem] Add enemy score to player");
                    sm.Score += enemy.Enemy.Score;
                }
        }
    }
}
