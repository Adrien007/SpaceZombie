//EnemyAttackManager.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Canons;
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
        private CanonEnemy canon;
        private EnemyFireService service;
        private Timer rateOfFire;

        public EnemyAttackManager(Control mainAera,
                                 IResetEtatNotifier resetEtatNotifier,
                                EnemyFireService service)
        {
            resetEtatNotifier.Register(this);
            enemiesAvailable = new List<Node2D>();
            this.service = service;
            PackedScene canonPrefab = GD.Load<PackedScene>("res://Prefabs/canon_enemy.tscn");
            canon = canonPrefab.Instantiate<CanonEnemy>();
            mainAera.AddChild(canon);
            canon.Initialize("projectile_enemy", new Projectile(1, 200f), resetEtatNotifier);

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
                canon.GlobalPosition = enemyFire[i].GlobalPosition;
                canon.Fire(GetGlobalDirection(enemyFire[i].GlobalRotation));
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
            canon.StopSound();
        }

        public void StartTimerState()
        {
            rateOfFire.Start();
        }
    }
}