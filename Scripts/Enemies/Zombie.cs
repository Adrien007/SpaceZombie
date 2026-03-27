using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;
using System;

namespace SpaceZombie.Enemies
{
    public partial class Zombie : BaseEnemy
    {
        [Export] float attackDistance = 200f;
        private bool isAttaking = false;
        private bool hasGrabbedPlayer = false;
        private float attackSpeed = 800;
        private float maxAngleRad;
        public float attackDistancePosition;

        public override void _Ready()
        {
            base._Ready();
            attackDistancePosition = GetViewportRect().Size.Y - attackDistance;
            maxAngleRad = Mathf.DegToRad(40);
        }

        public override void _Process(double delta)
        {
            if (hasGrabbedPlayer)
            {
                GlobalPosition = new Vector2(joueur.GlobalPosition.X, GlobalPosition.Y);
            }
            else if (isAttaking)
            {
                Position += velocity.Normalized() * attackSpeed * (float)delta;
            }
            else if (GlobalPosition.Y < attackDistancePosition)
            {
                Vector2 toTarget = joueur.GlobalPosition - GlobalPosition;
                MoveToTarget(ClampDirection(toTarget), delta);
                RotateTowardTarget(velocity, delta);
            }
            else if (animation.IsPlaying())
            {
                velocity = ClampDirection(joueur.GlobalPosition - GlobalPosition);
                RotateTowardTarget(velocity, delta);
            }
            else
            {
                animation.Play("prepare_attack");
            }
        }

        private void Attack()
        {
            isAttaking = true;
        }
        private Vector2 ClampDirection(Vector2 desiredDir)
        {
            float desiredAngle = Vector2.Down.AngleTo(desiredDir);
            if (Mathf.Abs(desiredAngle) > maxAngleRad)
            {
                float clampedAngle = Vector2.Down.Angle() + Mathf.Sign(desiredAngle) * maxAngleRad;
                return new Vector2(Mathf.Cos(clampedAngle), Mathf.Sin(clampedAngle));
            }
            return desiredDir;
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is IDamagable damagable)
            {
                if (!damagable.IsDodging)
                {
                    CallDeferred(nameof(PlayerGrabbed));
                }
            }
        }

        private void PlayerGrabbed()
        {
            Monitorable = false;
            Monitoring = false;
            hasGrabbedPlayer = true;
            velocity = Vector2.Zero;
            animation.Play("explode");
        }

        public void DamagePlayer()
        {
            joueur.TakeDamage(1);
        }
    }
}

