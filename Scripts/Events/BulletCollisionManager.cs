//BulletCollisionManager.cs
using System;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Utilitaires.Layers;
using SpaceZombie.Utilitaires.Tableaux;

namespace SpaceZombie.Events
{
    public interface IBulletCollisionManager
    {
        public void ReportCollision(ProjectileObjet projectile, Area2D aera2D);
    }
    public class BulletCollisionManager : IBulletCollisionNotifier, IBulletCollisionManager
    {
        private List<IBulletCollisionOberser> _observers = new();
        private CircularBuffer<(ProjectileObjet, Area2D)> _notificationBuffer;
        private IBulletCollisionService _service;
        public BulletCollisionManager(int bufferSize, IBulletCollisionService service)
        {
            _notificationBuffer = new CircularBuffer<(ProjectileObjet, Area2D)>(bufferSize);
            _service = service;
        }

        public void Register(IBulletCollisionOberser observer) => _observers.Add(observer);
        public void Unregister(IBulletCollisionOberser observer) => _observers.Remove(observer);

        private void Notify(ProjectileObjet projectile, Area2D aera2D)
        {
            foreach (var observer in _observers)
            {
                observer.OnBulletCollision(projectile, aera2D);
            }
        }
        public void Notify()
        {
            while (_notificationBuffer.CanDequeue())
            {
                var (projectile, area2D) = _notificationBuffer.Dequeue();
                if (_service.ProjectilHitLogic(projectile, area2D))
                {
                    Notify(projectile, area2D);
                }
            }
        }

        public void ReportCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            GD.Print("[CollisionManager] Bullet hit something.");
            _notificationBuffer.Enqueue((projectile, aera2D));
        }


        public static bool ProjectileHitEnemy(Area2D aera2D)
        {
            return (aera2D.CollisionLayer & LayerDictionnary.GetLayer(LayerDictionnary.Enemy)) != 0;
        }
        public static bool ProjectileHitPlayer(Area2D aera2D)
        {
            return (aera2D.CollisionLayer & LayerDictionnary.GetLayer(LayerDictionnary.Joueur)) != 0;
        }
    }


    public interface IBulletCollisionService
    {
        public bool ProjectilHitLogic(ProjectileObjet projectile, Area2D aera2D);
    }
    public class BulletCollisionOnEnemyService : IBulletCollisionService
    {
        public bool ProjectilHitLogic(ProjectileObjet projectile, Area2D aera2D)
        {
            if (!BulletCollisionManager.ProjectileHitEnemy(aera2D))
            {
                return false; //Projectile did not hit an enemy
            }

            if (projectile.Projectile.JusHitValidObjects)
            {
                return projectile.Projectile.CanHitMultipleObjects; //Projectile can hit multiple objects
            }

            projectile.Projectile.JusHitValidObjects = true; //Set to true to avoid hitting others objects again
            return true;

            /*if (BulletCollisionManager.ProjectileHitEnemy(aera2D))
            {
                if (projectile.Projectile.JusHitValidObjects)
                {
                    if (projectile.Projectile.CanHitMultipleObjects)
                    {
                        return true; //Projectile can hit multiple objects
                    }
                    else
                    {
                        return false; //Cannot hit multiple object
                    }
                }
                else
                {
                    projectile.Projectile.JusHitValidObjects = true; //Set to true to avoid hitting others objects again
                    return true; //Projectile can hit only one object
                }
            }
            return false; //Projectile did not hit an enemy*/
        }
    }
    public class BulletCollisionOnPlayerService : IBulletCollisionService
    {
        public bool ProjectilHitLogic(ProjectileObjet projectile, Area2D aera2D)
        {
            if (!BulletCollisionManager.ProjectileHitPlayer(aera2D))
            {
                return false; //Projectile did not hit player
            }
            if (projectile.Projectile.JusHitValidObjects)
            {
                return false; //Projectile cannot hit multiple time the player
            }

            projectile.Projectile.JusHitValidObjects = true; //Set to true to avoid hitting others objects again
            return true;
        }
    }
}