//CanonObjet.cs
using System.Collections.Generic;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Events;

namespace SpaceZombie.Cannons
{
    public partial class CanonObjet : Node2D
    {
        private Queue<ProjectileObjet> projectileBuffer;
        private int level = 0;
        PackedScene projectileScene;
        Node mainAera;
        Projectile projectile;
        IResetEtatNotifier resetEtatNotifier;
        private uint collisionLayer;


        public override void _Ready()
        {
            projectileScene = (PackedScene)ResourceLoader.Load("res://Prefabs/projectile_objet.tscn");
            mainAera = GetTree().CurrentScene;
            base._Ready();
        }
        public void Initialize(int level,
                                Projectile projectile, uint collisionLayer,
                                IResetEtatNotifier resetEtatNotifier)
        {
            this.level = level;
            this.projectile = projectile;
            this.resetEtatNotifier = resetEtatNotifier;
            this.collisionLayer = collisionLayer;
            InitializeBuffer(level, projectile, resetEtatNotifier);
        }
        private void InitializeBuffer(int level,
                                        Projectile projectile,
                                        IResetEtatNotifier resetEtatNotifier)
        {
            projectileBuffer = new Queue<ProjectileObjet>();
            for (int i = 0; i <= level; i++)
            {
                projectileBuffer.Enqueue(newProjectile());
            }
        }

        public void LevelUp()
        {
            level += 1;
            projectile.AugmenteVitesse(0.1f);
        }

        public Vector2 GetGlobalDirection()
        {
            return new Vector2(Mathf.Cos(GlobalRotation), Mathf.Sin(GlobalRotation)).Normalized();
        }

        public void Fire()
        {
            int projectilePosition = level * 20;
            for (int i = 0; i <= level; i++)
            {
                Vector2 globalPosition = GlobalPosition;
                globalPosition.X += projectilePosition;
                getNextProjectile().Fire(GetGlobalDirection(), globalPosition, GlobalRotation);
                projectilePosition -= 40;
            }
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
            projectileInstance.Initialize(projectile, collisionLayer, resetEtatNotifier);
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