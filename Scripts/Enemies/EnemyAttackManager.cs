//EnemyAttackManager.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
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
    public class EnemyAttackManager : IEnemyAttackManagerSetEnemy
    {
        private List<Node2D> enemiesAvailable;
        private CanonObjet cannon0;
        private EnemyFireService service;
        private Timer rateOfFire;

        public EnemyAttackManager(Control mainAera, int capacity, uint collisionLayer, uint collisionMask, Projectile projectile, IBulletCollisionManager bulletCollisionManager,
                                EnemyFireService service)
        {
            enemiesAvailable = new List<Node2D>();
            this.service = service;
            PackedScene cannonPrefab = GD.Load<PackedScene>("res://Prefabs/cannon.tscn");
            cannon0 = cannonPrefab.Instantiate<CanonObjet>();
            mainAera.AddChild(cannon0);
            cannon0.Rotate(Mathf.Pi);
            cannon0.Initialize(mainAera, capacity, collisionLayer, collisionMask, projectile, bulletCollisionManager);

            rateOfFire = service.GetTimerRateOfFire();
            mainAera.AddChild(rateOfFire);
            rateOfFire.Timeout += Fire;
            rateOfFire.Start();
        }

        private void Fire()
        {
            EnemyFireService.UpdateEnemyAvailable(enemiesAvailable);
            List<Node2D> enemyFire = service.PickRandom(enemiesAvailable);
            if (enemyFire.Count > 0)
            {
                cannon0.GlobalPosition = enemyFire[0].GlobalPosition;
                cannon0.Fire();
            }
        }

        public void SetEnemyForLevel(List<Node2D> allEnemy)
        {
            this.enemiesAvailable = allEnemy;
        }
    }
}