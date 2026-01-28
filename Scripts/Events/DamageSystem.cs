//DamageSystem.cs
using System.Formats.Asn1;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using SpaceZombie.Joueurs;

namespace SpaceZombie.Events
{
    public class DamageSystem : IBulletCollisionOberser
    {
        public virtual void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            // // Defer the call to Disable() to avoid issues during signal processing
            // projectile.Disable();
            // //projectile.CallDeferred(ProjectileObjet.CALL_DEFFERED_DISABLE);
            // EnemyObjet enemy = EnemyEventHandler.TrouverParentEnemy(aera2D);
            // EnemyEventHandler.GestionHitSurEnemy(enemy, projectile.Projectile.Damage);
            // GD.Print($"[DamageSystem] Enemy took damage!");
            throw new System.NotImplementedException("OnBulletCollision method must be overridden in derived classes.");
        }
    }

    public class BulletDamageSystem : IBulletCollisionOberser//DamageSystem
    {
        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            projectile.Disable();
            //projectile.CallDeferred(ProjectileObjet.CALL_DEFFERED_DISABLE);
            EnemyObjet enemy = EnemyEventHandler.TrouverParent(aera2D);
            EnemyEventHandler.GestionHitSurEnemy(enemy, projectile.Projectile.Damage);
            GD.Print($"[DamageSystem] Enemy took damage!");
        }
    }

    public class PlayerDamageSystem : IBulletCollisionOberser
    {
        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            projectile.Disable();
            //projectile.CallDeferred(ProjectileObjet.CALL_DEFFERED_DISABLE);
            var j = JoueurEventHandler.TrouverParent(aera2D);
            JoueurEventHandler.GestionHitSurEnemy(j, projectile.Projectile.Damage);
            GD.Print($"[DamageSystem] Enemy took damage!");
        }
    }
}