//CanonObjet.cs
using System;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Events;
using SpaceZombie.Mondes.Utilitaires;

namespace SpaceZombie.Canons
{
    public partial class CanonObjet : Node2D
    {
        private Queue<ProjectileObjet> projectileBuffer;
        PackedScene projectileScene;
        Projectile projectile;
        Node mainAera;

        public override void _Ready()
        {
            mainAera = GetTree().CurrentScene.FindChild("MainAera");
            base._Ready();
        }
        public void Initialize(string projectileName,
                                Projectile projectile)
        {
            this.projectile = projectile;
            projectileScene = (PackedScene)ResourceLoader.Load($"res://Prefabs/{projectileName}.tscn");
            CallDeferred("InitializeBuffer");
        }
        private void InitializeBuffer()
        {
            projectileBuffer = new Queue<ProjectileObjet>();
            for (int i = 0; i <= 8; i++)
            {
                projectileBuffer.Enqueue(NewProjectile());
            }
        }
        public virtual void Fire(Vector2 direction)
        {
            GetNextProjectile().Fire(direction, GlobalPosition, GlobalRotation);
        }

        private ProjectileObjet GetNextProjectile()
        {
            if (projectileBuffer.Count > 0)
            {
                return projectileBuffer.Dequeue();
            }
            else
            {
                return NewProjectile();
            }
        }

        private ProjectileObjet NewProjectile()
        {
            ProjectileObjet projectileInstance = (ProjectileObjet)projectileScene.Instantiate();
            projectileInstance.Initialize(projectile);
            projectileInstance.OutOfBoundignal += HandleOutOfBoundSignal;
            mainAera.AddChild(projectileInstance);
            return projectileInstance;
        }

        private void HandleOutOfBoundSignal(ProjectileObjet projectileObj)
        {
            //GD.Print("HandleOutOfBoundSignal + " + projectileObj.Name);
            projectileBuffer.Enqueue(projectileObj);
        }
    }
}
