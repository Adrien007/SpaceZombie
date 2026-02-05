//EnemyAttackManager.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
using System;

namespace SpaceZombie.Enemies
{
    /// <summary>
    /// At each level, get all eneymy. If enemy are visdible, thay can posstentially attack.
    /// </summary>
    public class EnemyAttackManager
    {
        private Node2D[] allEnemy;
        private CanonObjet cannon0;

        public EnemyAttackManager(Control mainAera, int capacity, uint collisionLayer, uint collisionMask, Projectile projectile, IBulletCollisionManager bulletCollisionManager)
        { 
            allEnemy = Array.Empty<Node2D>();
            //res://Prefabs/cannon.tscn
            PackedScene cannonPrefab = GD.Load<PackedScene>("res://Prefabs/cannon.tscn");
            cannon0 = cannonPrefab.Instantiate<CanonObjet>();
            mainAera.AddChild(cannon0);
            cannon0.Rotate(Mathf.Pi);
            cannon0.Initialize(mainAera, capacity, collisionLayer, collisionMask, projectile, bulletCollisionManager);
        }

        int i = -30;
        public void Fire()
        {
            if (i++ > 30)
            {
                i = 0;
                cannon0.Fire();
            }
        }

        public void SetEnemyForLevel(Node2D[] allEnemy)
        {
            this.allEnemy = allEnemy;
        }

        private void BBB()
        {
            //Selectionner un enemy random a tirer missile.
        }
    }
}