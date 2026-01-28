//EnemyEventSystem.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Events;
using SpaceZombie.Scores.GameScore;

namespace SpaceZombie.Enemies
{
    public class EnemyEventSystem : IBulletCollisionManager
    {
        private BulletCollisionManager bcm;

        public EnemyEventSystem(BulletCollisionManager bulletCollisionManager)
        {
            bcm = bulletCollisionManager;
        }
        public void Register(ZombiesSpawn zombiesSpawn,
                                EndLevelSystem endLevelSystem)
        {
            //Toujours dans cette ordre! Ordre important!
            bcm.Register(new BulletDamageSystem());
            bcm.Register(new SoundSystemEnemy());
            bcm.Register(new ScoreSystem(new ScoreManager(0)));
            bcm.Register(new InLigneSpawnerUtilitiesEvent(zombiesSpawn, new InLigneSpawnerUtilitiesEventService()));
            bcm.Register(endLevelSystem);
        }

        public void ReportCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            bcm.ReportCollision(projectile, aera2D);
        }
        public void Notify()
        {
            bcm.Notify();
        }
    }
}