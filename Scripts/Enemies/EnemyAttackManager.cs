//EnemyAttackManager.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
using System;
using System.Collections.Generic;

namespace SpaceZombie.Enemies
{
    public interface IEnemyAttackManagerSetEnemy
    {
        public void SetEnemyForLevel(List<Node2D> allEnemy);
    }
    /// <summary>
    /// At each level, get all eneymy. If enemy are visdible, thay can posstentially attack.
    /// </summary>
    public class EnemyAttackManager : IEnemyAttackManagerSetEnemy, IResetEtatObserver
    {
        private List<Node2D> enemiesAvailable;
        private CanonObjet cannon0;
        private EnemyFireService service;
        private Timer rateOfFire;

        public EnemyAttackManager(Control mainAera, int capacity, uint collisionLayer, uint collisionMask, Projectile projectile, 
                                IBulletCollisionManager bulletCollisionManager, IResetEtatNotifier resetEtatNotifier,
                                EnemyFireService service)
        {
            resetEtatNotifier.Register(this);
            enemiesAvailable = new List<Node2D>();
            this.service = service;
            PackedScene cannonPrefab = GD.Load<PackedScene>("res://Prefabs/cannon.tscn");
            cannon0 = cannonPrefab.Instantiate<CanonObjet>();
            mainAera.AddChild(cannon0);
            cannon0.Rotate(Mathf.Pi);
            cannon0.Initialize(mainAera, capacity, collisionLayer, collisionMask, projectile, bulletCollisionManager, resetEtatNotifier);

            rateOfFire = service.GetTimerRateOfFire();
            mainAera.AddChild(rateOfFire);
            rateOfFire.Timeout += Fire;
        }

        private void Fire()
        {
            EnemyFireService.UpdateEnemyAvailable(enemiesAvailable);
            List<Node2D> enemyFire = service.PickRandom(enemiesAvailable);
            for (int i = 0; i < enemyFire.Count; i++)
            {
                cannon0.GlobalPosition = enemyFire[i].GlobalPosition;
                cannon0.Fire();
            }
        }

        public void SetEnemyForLevel(List<Node2D> allEnemy)
        {
            this.enemiesAvailable = allEnemy;
        }

        public void OnResetToInitaialState()
        {
            rateOfFire.Stop();
        }

        public void StartTimerState()
        {
            rateOfFire.Start();
        }
    }
}