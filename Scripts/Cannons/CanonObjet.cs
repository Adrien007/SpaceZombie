//CanonObjet.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Events;
using SpaceZombie.Utilitaires.Tableaux;

namespace SpaceZombie.Cannons
{
    public partial class CanonObjet : Node2D
    {
        private CircularBuffer<ProjectileObjet> projectileBuffer;
        public override void _Ready()
        {
            base._Ready();
            
        }
        public void Initialize(Control mainAera, int capacity, Projectile projectile, IBulletCollisionManager bulletCollisionManager)
        {
            InitializeBuffer(mainAera, capacity, projectile, bulletCollisionManager);
        }
        private void InitializeBuffer(Control mainAera, int capacity, Projectile projectile, IBulletCollisionManager bulletCollisionManager)
        {
            projectileBuffer = new CircularBuffer<ProjectileObjet>(capacity);

            // Load the scene dynamically by its path at runtime.
            PackedScene projectileScene = (PackedScene)ResourceLoader.Load("res://Prefabs/projectile_objet.tscn");

            // Populate the buffer with preloaded projectiles.
            for (int i = 0; i < capacity; i++)
            {
                ProjectileObjet projectileInstance = (ProjectileObjet)projectileScene.Instantiate();
                projectileInstance.Initialize(projectile, bulletCollisionManager);
                projectileInstance.HitSignal += HandleHitSignal;
                mainAera.AddChild(projectileInstance);
                projectileBuffer.Enqueue(projectileInstance);
            }
        }

        public Vector2 GetGlobalDirection()
        {
            return new Vector2(Mathf.Cos(GlobalRotation), Mathf.Sin(GlobalRotation)).Normalized();
        }

        public void Fire()
        {
            if (!projectileBuffer.CanDequeue())
            {
                    GD.Print("No available projectiles to fire!");
                    return;
            }

            // Get the next available projectile.
            ProjectileObjet nextProjectile = projectileBuffer.Dequeue();
            nextProjectile.Fire(GetGlobalDirection(), GlobalPosition, GlobalRotation);
        }
        private void HandleHitSignal(ProjectileObjet projectileObj)
        {
            //GD.Print("HandleHitSignal");
            projectileBuffer.Enqueue(projectileObj);
        }
    }
}