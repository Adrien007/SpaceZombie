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
        private float freeZoneWidth = 200f;
        private float fireBulletDelay = 2.0f;
        private RandomNumberGenerator random = new RandomNumberGenerator();
        private Timer fireBulletsTimer = new Timer();
        private int repeatAttack = 0;
        private Action attackEndListener;
        private float lazerZoneWidth;
        private float lazerZoneDelay;

        public override void _Ready()
        {
            lazerZoneWidth = 200;
            lazerZoneDelay = 3;
            fireBulletsTimer.WaitTime = fireBulletDelay;
            fireBulletsTimer.Timeout += FireBullet;
            AddChild(fireBulletsTimer);
        }

        public void Initialize(Joueur joueur)
        {
            this.joueur = joueur;
            for (int i = 0; i < 5; i++)
            {
                CanonObjet canon = (CanonObjet)FindChild($"Canon{i}");
                canon.Initialize("projectile_enemy", new Projectile(1, 400f));
                LazerRay lazerRay = canon.GetChild<LazerRay>(0);
                lazers.Add(lazerRay);
                canons.Add(canon);
            }

            lazers[4].fireEndedListener = LazerAttackEnded;
            lazerZone.attackEndedListener = ZoneAttackEnded;
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
                lazerZone.Fire(lazerZoneWidth, lazerZoneDelay);
            }
            else
            {
                attackEndListener();
            }
        }

        private void FireBullet()
        {
            CanonObjet canon = canons[random.RandiRange(0, 4)];
            canon.Fire((joueur.GlobalPosition - canon.GlobalPosition).Normalized());
            bulletSound.Play();
        }

        public void FireBullets()
        {
            if (fireBulletsTimer.IsStopped())
            {
                FireBullet();
                fireBulletsTimer.Start(fireBulletDelay);
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
            lazerZone.Fire(lazerZoneWidth, lazerZoneDelay);
        }
    }

}
