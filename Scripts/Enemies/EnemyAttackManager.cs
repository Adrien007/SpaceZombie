//EnemyAttackManager.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
using System.Collections.Generic;

namespace SpaceZombie.Enemies
{
    /// <summary>
    /// At each level, get all eneymy. If enemy are visdible, thay can posstentially attack.
    /// </summary>
    public class EnemyAttackManager
    {
        private List<Node2D> enemiesAvailable;
        private CanonObjet cannon0;
        private EnemyFireService service;

        public EnemyAttackManager(Control mainAera, int capacity, uint collisionLayer, uint collisionMask, Projectile projectile, IBulletCollisionManager bulletCollisionManager,
                                EnemyFireService service)
        {
            enemiesAvailable = new List<Node2D>();
            this.service = service;
            //res://Prefabs/cannon.tscn
            PackedScene cannonPrefab = GD.Load<PackedScene>("res://Prefabs/cannon.tscn");
            cannon0 = cannonPrefab.Instantiate<CanonObjet>();
            mainAera.AddChild(cannon0);
            cannon0.Rotate(Mathf.Pi);
            cannon0.Initialize(mainAera, capacity, collisionLayer, collisionMask, projectile, bulletCollisionManager);
        }

        int i = -60;
        public void Fire()
        {
            if (i++ > 30)
            {
                i = 0;
                EnemyFireService.UpdateEnemyAvailable(enemiesAvailable);
                List<Node2D> enemyFire = service.PickRandom(enemiesAvailable);
                cannon0.GlobalPosition = enemyFire[0].GlobalPosition;
                cannon0.Fire();
            }
        }

        public void SetEnemyForLevel(List<Node2D> allEnemy)
        {
            GD.Print(allEnemy.Count);
            this.enemiesAvailable = allEnemy;
        }

        private void SelectAvailableEnemyToFire(int nbElementsToSelect)
        {
            //Selectionner un enemy random a tirer missile.
            
        }
    }
}