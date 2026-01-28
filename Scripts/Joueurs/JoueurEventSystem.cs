//JoueurEventSystem.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Events;

namespace SpaceZombie.Joueurs
{
    public class JoueurEventSystem : IBulletCollisionManager
    {
        private BulletCollisionManager bcm;

        public JoueurEventSystem(BulletCollisionManager bulletCollisionManager)
        {
            bcm = bulletCollisionManager;
        }
        public void Register(EndLevelSystemPlayer endLevelSystem)
        {
            //Toujours dans cette ordre! Ordre important!
            bcm.Register(new PlayerDamageSystem());
            bcm.Register(new SoundSystemJoueur());
            //bcm.Register(new ScoreSystem(new ScoreManager(0)));
            //bcm.Register(new InLigneSpawnerUtilitiesEvent(zombiesSpawn, new InLigneSpawnerUtilitiesEventService()));
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