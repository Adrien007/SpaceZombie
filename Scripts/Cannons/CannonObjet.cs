//CanonObjet.cs
using System;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Events;
using SpaceZombie.Mondes.Utilitaires;

namespace SpaceZombie.Cannons
{
    public partial class CannonObjet : Node2D
    {
        private Queue<ProjectileObjet> projectileBuffer;
        PackedScene projectileScene;
        Node mainAera;
        Projectile projectile;
        IResetEtatNotifier resetEtatNotifier;

        public override void _Ready()
        {
            base._Ready();
        }
        public void Initialize(int level, string projectileName,
                                Projectile projectile,
                                IResetEtatNotifier resetEtatNotifier)
        {
            mainAera = GetTree().CurrentScene.FindChild("MainAera");
            this.projectile = projectile;
            this.resetEtatNotifier = resetEtatNotifier;
            projectileScene = (PackedScene)ResourceLoader.Load($"res://Prefabs/{projectileName}.tscn");
            InitializeBuffer();
        }
        private void InitializeBuffer()
        {
            projectileBuffer = new Queue<ProjectileObjet>();
            for (int i = 0; i <= 15; i++)
            {
                projectileBuffer.Enqueue(newProjectile());
            }
        }
        public void Fire(Vector2 direction)
        {
            getNextProjectile().Fire(direction, GlobalPosition, GlobalRotation);
        }

        private ProjectileObjet getNextProjectile()
        {
            if (projectileBuffer.Count > 0)
            {
                return projectileBuffer.Dequeue();
            }
            else
            {
                return newProjectile();
            }
        }

        private ProjectileObjet newProjectile()
        {
            ProjectileObjet projectileInstance = (ProjectileObjet)projectileScene.Instantiate();
            projectileInstance.Initialize(projectile, resetEtatNotifier);
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