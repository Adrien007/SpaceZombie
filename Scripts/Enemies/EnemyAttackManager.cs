//EnemyAttackManager.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
using SpaceZombie.Utilitaires.Layers;
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
        private CannonEnemy cannon0;
        private EnemyFireService service;
        private Timer rateOfFire;

        public EnemyAttackManager(Control mainAera,
                                 IResetEtatNotifier resetEtatNotifier,
                                EnemyFireService service)
        {
            resetEtatNotifier.Register(this);
            enemiesAvailable = new List<Node2D>();
            this.service = service;
            PackedScene cannonPrefab = GD.Load<PackedScene>("res://Prefabs/cannon_enemy.tscn");
            cannon0 = cannonPrefab.Instantiate<CannonEnemy>();
            mainAera.AddChild(cannon0);
            cannon0.Initialize("projectile_enemy", new Projectile(1, 200f), resetEtatNotifier);

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
                cannon0.Fire(GetGlobalDirection(enemyFire[i].GlobalRotation));
            }
        }

        private Vector2 GetGlobalDirection(float globalRotation)
        {
            return new Vector2(Mathf.Cos(globalRotation), Mathf.Sin(globalRotation)).Normalized();
        }

        public void SetEnemyForLevel(List<Node2D> allEnemy)
        {
            this.enemiesAvailable = allEnemy;
        }

        public void OnResetToInitaialState()
        {
            rateOfFire.Stop();
            cannon0.StopSound();
        }

        public void StartTimerState()
        {
            rateOfFire.Start();
        }
    }
}