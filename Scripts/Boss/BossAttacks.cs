using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Canons;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using System;
using System.Collections.Generic;

namespace SpaceZombie.Boss
{
    public partial class BossAttacks : Panel
    {
        [Export] public Joueur joueur;
        private List<CanonObjet> canons = new List<CanonObjet>();
        private List<LazerRay> lazers = new List<LazerRay>();
        [Export] private BossLazerZone lazerZone;
        [Export] private AudioStreamPlayer bulletSound;
        private float attackZoneMargin = 40f;
        private RandomNumberGenerator random = new RandomNumberGenerator();
        private Timer fireBulletsTimer = new Timer();
        private int repeatAttack = 0;
        private Action attackEndListener;
        public Projectile bullet;
        public float fireBulletDelay = 2.0f;
        public int numberOfBullet = 1;
        public float lazerZoneWidth;
        public float lazerZoneSpeed;

        public override void _Ready()
        {
            lazerZoneWidth = 200;
            lazerZoneSpeed = 1;
            bullet = new Projectile(1, 300f);

            fireBulletsTimer.WaitTime = fireBulletDelay;
            fireBulletsTimer.Timeout += FireAllBullet;
            AddChild(fireBulletsTimer);
        }

        public void Initialize(Joueur joueur)
        {
            this.joueur = joueur;
            for (int i = 0; i < 5; i++)
            {
                CanonObjet canon = (CanonObjet)FindChild($"Canon{i}");
                canon.Initialize("projectile_enemy", bullet);
                LazerRay lazerRay = canon.GetChild<LazerRay>(0);
                lazers.Add(lazerRay);
                canons.Add(canon);
            }

            lazers[4].fireEndedListener = LazerAttackEnded;
            lazerZone.attackEndedListener = ZoneAttackEnded;
        }

        public void Phase2()
        {
            Stop();
            bullet.UpgradeVitesse(0.5f);
            fireBulletDelay = 1.0f;
            numberOfBullet = 2;
            lazerZoneWidth = 100;
            lazerZoneSpeed = 1.5f;
        }

        public void Phase3()
        {
            Stop();
            bullet.UpgradeVitesse(0.5f);
            fireBulletDelay = 1f;
            numberOfBullet = 4;
            lazerZoneWidth = 75;
            lazerZoneSpeed = 2.0f;
        }

        public void RegisterOnAttackEnd(Action onAttackEnded)
        {
            attackEndListener = onAttackEnded;
        }

        public void Stop()
        {
            fireBulletsTimer.Stop();
            lazerZone.Stop();
            foreach (LazerRay lazerRay in lazers)
            {
                lazerRay.Stop();
            }
        }

        private void LazerAttackEnded()
        {
            repeatAttack -= 1;
            if (repeatAttack > 0)
            {
                _FireRayLazers();
            }
            else
            {
                attackEndListener();
            }
        }
        private void ZoneAttackEnded()
        {
            repeatAttack -= 1;
            if (repeatAttack > 0)
            {
                lazerZone.Fire(lazerZoneWidth, lazerZoneSpeed);
            }
            else
            {
                attackEndListener();
            }
        }

        private void FireAllBullet()
        {
            foreach (CanonObjet canon in canons)
            {
                canon.Fire((joueur.GlobalPosition - canon.GlobalPosition).Normalized());
            }
            bulletSound.Play();
        }

        private void FireBulletSlowly()
        {
            CanonObjet canon = canons[random.RandiRange(0, 4)];
            canon.Fire((joueur.GlobalPosition - canon.GlobalPosition).Normalized());
            bulletSound.Play();
        }


        public void FireAllBullets()
        {
            if (fireBulletsTimer.IsStopped())
            {
                ClearFireBulletTimerListeners();
                fireBulletsTimer.Timeout += FireAllBullet;
                FireAllBullet();
                fireBulletsTimer.Start(fireBulletDelay);
            }
        }

        public void FireOneBulletAtTime()
        {
            if (fireBulletsTimer.IsStopped())
            {
                ClearFireBulletTimerListeners();
                fireBulletsTimer.Timeout += FireBulletSlowly;
                FireBulletSlowly();
                fireBulletsTimer.Start(fireBulletDelay);
            }
        }

        private void ClearFireBulletTimerListeners()
        {
            var callable = Callable.From(FireAllBullet);
            if (fireBulletsTimer.IsConnected("timeout", callable))
            {
                fireBulletsTimer.Disconnect("timeout", callable);
            }

            callable = Callable.From(FireBulletSlowly);
            if (fireBulletsTimer.IsConnected("timeout", callable))
            {
                fireBulletsTimer.Disconnect("timeout", callable);
            }
        }

        public void StopFireBullets()
        {
            fireBulletsTimer.Stop();
        }

        public void FireRayLazers(int repeat)
        {
            repeatAttack = repeat;
            _FireRayLazers();
        }

        private void _FireRayLazers()
        {
            foreach (LazerRay lazer in lazers)
            {
                lazer.Aim();
            }
        }

        public void FireZoneLazer(int repeat)
        {
            repeatAttack = repeat;
            lazerZone.Fire(lazerZoneWidth, lazerZoneSpeed);
        }
    }

}
