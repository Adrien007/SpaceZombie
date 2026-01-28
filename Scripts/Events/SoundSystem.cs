//SoundSystem.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires.Layers;

namespace SpaceZombie.Events
{
    // public class SoundSystem : IBulletCollisionOberser
    // {
    //     public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
    //     {
    //         EnemyObjet enemy = EnemyEventHandler.TrouverParentEnemy(aera2D);
    //         if (EnemyEventHandler.IsEnemyDie(enemy) && !EnemyEventHandler.IsDeadSoundPlayed(enemy))
    //         {
    //             enemy.enemyFlagLogic.deadSoundPlayed = true;
    //             GD.Print("[SoundSystem] Play 'enemy Die' sound.");
    //         }
    //         else
    //         {
    //             GD.Print("[SoundSystem] Play 'enemy hit' sound.");
    //         }
    //     }
    // }
    public class SoundSystemEnemy : IBulletCollisionOberser
    {
        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            EnemyObjet enemy = EnemyEventHandler.TrouverParent(aera2D);
            if (EnemyEventHandler.IsEnemyDie(enemy) && !EnemyEventHandler.IsDeadSoundPlayed(enemy))
            {
                enemy.enemyFlagLogic.deadSoundPlayed = true;
                GD.Print("[SoundSystemEnemy] Play 'enemy Die' sound.");
            }
            else
            {
                GD.Print("[SoundSystemEnemy] Play 'enemy hit' sound.");
            }
        }
    }
    public class SoundSystemJoueur : IBulletCollisionOberser
    {
        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            var obj = JoueurEventHandler.TrouverParent(aera2D);
            if (JoueurEventHandler.IsObjetDie(obj.jState) && !JoueurEventHandler.IsDeadSoundPlayed(obj.jState))
            {
                obj.jState.DeadSoundPlayed = true;
                GD.Print("[SoundSystemJoueur] Play 'player Die' sound.");
            }
            else if (!JoueurEventHandler.IsObjetInvincible(obj.jState))
            {
                GD.Print("[SoundSystemJoueur] Play 'player hit' sound.");
            }
        }
    }
}
