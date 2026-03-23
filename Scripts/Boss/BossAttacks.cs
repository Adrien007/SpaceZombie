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
        private List<BossLazerRay> lazers = new List<BossLazerRay>();
        [Export] private BossLazerZone attackZone1;
        [Export] private BossLazerZone attackZone2;
        private float attackZoneMargin = 40f;
        private float freeZoneWidth = 200f;
        private float fireBulletDelay = 2.0f;
        private RandomNumberGenerator random = new RandomNumberGenerator();
        private Timer fireBulletsTimer = new Timer();
        private int repeatAttack = 0;
        private Action attackEndListener;
        private Timer rayLazerDamageTimer = new Timer();

        public override void _Ready()
        {
            fireBulletsTimer.WaitTime = fireBulletDelay;
            fireBulletsTimer.Timeout += FireBullet;
            AddChild(fireBulletsTimer);
            rayLazerDamageTimer.OneShot = true;
            AddChild(rayLazerDamageTimer);
        }

        public void Initialize(Joueur joueur)
        {
            this.joueur = joueur;
            for (int i = 0; i < 5; i++)
            {
                CanonObjet canon = (CanonObjet)FindChild($"Canon{i}");
                canon.Initialize("projectile_enemy", new Projectile(1, 200f));
                BossLazerRay lazerRay = canon.GetChild<BossLazerRay>(0);
                lazerRay.lazerCollideListener = onLazerCollide;
                lazers.Add(lazerRay);
                canons.Add(canon);
            }

            lazers[0].animation.AnimationFinished += LazerAttackEnded;
            attackZone1.animation.AnimationFinished += ZoneAttackEnded;
        }

        public void RegisterOnAttackEnd(Action onAttackEnded)
        {
            attackEndListener = onAttackEnded;
        }

        private void onLazerCollide()
        {
            if (rayLazerDamageTimer.TimeLeft == 0)
            {
                joueur.TakeDamage(1);
                rayLazerDamageTimer.Start();
            }
        }

        private void LazerAttackEnded(StringName animName)
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
        private void ZoneAttackEnded(StringName animName)
        {
            repeatAttack -= 1;
            if (repeatAttack > 0)
            {
                _FireZoneLazer();
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
            foreach (BossLazerRay lazer in lazers)
            {
                lazer.StartFire();
            }
        }

        public void FireZoneLazer(int repeat)
        {
            repeatAttack = repeat;
            _FireZoneLazer();
        }

        private void _FireZoneLazer()
        {
            attackZone1.StopAnimation();
            attackZone2.StopAnimation();
            Vector2 screenSize = GetViewportRect().Size;
            float freeZonePosition = random.RandfRange(attackZoneMargin, screenSize.X - attackZoneMargin - freeZoneWidth);
            attackZone1.Fire(new Vector2(freeZonePosition, screenSize.Y), 0);
            attackZone2.Fire(new Vector2(screenSize.X - freeZonePosition - freeZoneWidth, screenSize.Y), freeZonePosition + freeZoneWidth);
        }
    }

}
