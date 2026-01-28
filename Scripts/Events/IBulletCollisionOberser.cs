//IBulletCollisionOberser.cs
using Godot;
using SpaceZombie.Ammunitions;

namespace SpaceZombie.Events
{
    public interface IBulletCollisionOberser
    {
        void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D);
    }

    public interface IBulletCollisionNotifier
    {
        void Register(IBulletCollisionOberser observer);
        void Unregister(IBulletCollisionOberser observer);
        //void Notify(ProjectileObjet projectile, Area2D aera2D);
    }
}