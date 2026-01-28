//EndLevelSystem.cs
using System;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using SpaceZombie.Joueurs;

namespace SpaceZombie.Events
{
    public interface IEndLevelSystem
    {
        public delegate void EndLevelSignalEventHandler();
        public event EndLevelSignalEventHandler EndLevelSignal;
    }
    public interface INbEnemy
    {
        public int NbEnemy { get; set; }
    }
    public class EndLevelSystem : IBulletCollisionOberser, IEndLevelSystem, INbEnemy
    {
        public event IEndLevelSystem.EndLevelSignalEventHandler EndLevelSignal;

        private int nbEnemy;
        public int NbEnemy { get => nbEnemy; set => nbEnemy = Math.Abs(value); }


        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            EnemyObjet enemy = EnemyEventHandler.TrouverParent(aera2D);
            if (EnemyEventHandler.IsEnemyDie(enemy) && !EnemyEventHandler.IsEndLevelAdded(enemy))
            {
                enemy.enemyFlagLogic.endLevelAdded = true;
                nbEnemy -= 1;
                GD.Print($"[EndLevelSystem] Remaining enemies: {nbEnemy}");
                if (nbEnemy <= 0)
                {
                    EndLevelSignal.Invoke();
                }
            }
        }
    }
    
    public class EndLevelSystemPlayer : IBulletCollisionOberser, IEndLevelSystem
    {
        public event IEndLevelSystem.EndLevelSignalEventHandler EndLevelSignal;

        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            var j = JoueurEventHandler.TrouverParent(aera2D);
            if (JoueurEventHandler.IsObjetDie(j.jState) && !JoueurEventHandler.IsEndLevel(j.jState))
            {
                j.jState.EndLevel = true; 
                EndLevelSignal.Invoke();
            }
        }
    }
}