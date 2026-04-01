using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;
using System;

namespace SpaceZombie.Enemies
{
    public partial class Zombie1 : BaseEnemy
    {
        [Export] float attackDistance = 200f;
        private float maxAngleRad;

        public override void _Ready()
        {
            base._Ready();
            ((MoveInState)currentState).distancePosition = GetViewportRect().Size.Y - attackDistance;
            maxAngleRad = Mathf.DegToRad(40);
            this.score = 220;
        }

        public override Vector2 GetJoueurDirection()
        {
            return ClampDirection(joueur.GlobalPosition - GlobalPosition);
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

        public void PlayerGrabbed()
        {
            Monitorable = false;
            Monitoring = false;
            GlobalPosition = new Vector2(joueur.GlobalPosition.X, GlobalPosition.Y);
            direction = Vector2.Zero;
            animation.Play("explode");
        }

        public void StickToJoueur()
        {
            GlobalPosition = new Vector2(joueur.GlobalPosition.X, GlobalPosition.Y);
        }

        public void DamagePlayer()
        {
            joueur.TakeDamage(1);
        }
    }
}

