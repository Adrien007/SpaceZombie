using Godot;
using SpaceZombie.Utilitaires;
using System;
namespace SpaceZombie.Ammunitions
{
    public partial class LazerRay : Area2D
    {
        [Export] int damage;
        [Export] RayCast2D aimRay;
        [Export] Line2D lazerMain;
        [Export] Line2D lazerGlow;
        [Export] Line2D lazerCore;
        [Export] Timer fireDelay;
        [Export] Timer fireTime;
        [Export] Timer repeatDamage;
        [Export] AudioStreamPlayer lazerSound;
        public Action fireEndedListener;
        private IDamagable damaging;
        private Tween fireTween;

        public bool isAiming { get => aimRay.Visible; }
        public override void _Ready()
        {
            SetPhysicsProcess(false);
            fireDelay.Timeout += Fire;
            fireTime.Timeout += Stop;
            repeatDamage.Timeout += RepeatDamage;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (aimRay.IsColliding() && fireDelay.IsStopped())
            {
                fireDelay.Start();
            }
        }

        public void Aim(bool withColllistionDetection = false)
        {
            aimRay.Visible = true;
            if (withColllistionDetection)
            {
                aimRay.Enabled = true;
                SetPhysicsProcess(true);
                aimRay.ForceRaycastUpdate();
            }
            else
            {
                fireDelay.Start();
            }
        }

        private void Fire()
        {
            aimRay.Enabled = false;
            SetPhysicsProcess(false);
            aimRay.ForceRaycastUpdate();

            lazerMain.Visible = true;
            lazerGlow.Visible = true;
            lazerCore.Visible = true;
            Monitoring = true;

            lazerSound.Play();

            fireTween = CreateTween().SetLoops();
            fireTween.TweenProperty(lazerMain, "width", 25, 0.8);
            fireTween.Parallel().TweenProperty(lazerGlow, "width", 30, 0.8);
            fireTween.Parallel().TweenProperty(lazerCore, "width", 12, 0.8);
            fireTween.Chain().TweenProperty(lazerMain, "width", 16, 0.8);
            fireTween.Parallel().TweenProperty(lazerGlow, "width", 20, 0.8);
            fireTween.Parallel().TweenProperty(lazerCore, "width", 6, 0.8);
            fireTime.Start();
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is IDamagable damagable && !damagable.IsDodging)
            {
                damaging = damagable;
                damagable.TakeDamage(damage);
                repeatDamage.Start();
            }
        }

        private void OnAreaExited(Area2D area)
        {
            repeatDamage.Stop();
            if (area == damaging && !damaging.IsDodging)
            {
                damaging = null;
            }
        }

        private void RepeatDamage()
        {
            damaging.TakeDamage(damage);
        }

        public void Stop()
        {
            if (isAiming)
            {
                lazerSound.Stop();
                Monitoring = false;
                SetPhysicsProcess(false);
                repeatDamage.Stop();
                fireTween?.Kill();
                fireDelay.Stop();
                aimRay.Enabled = false;
                aimRay.Visible = false;
                lazerMain.Visible = false;
                lazerGlow.Visible = false;
                lazerCore.Visible = false;
                fireEndedListener?.Invoke();
            }

        }
    }
}


