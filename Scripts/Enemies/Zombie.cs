using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;
using System;

namespace SpaceZombie.Enemies
{
    public partial class Zombie : Area2D, IDamagable
    {
        [Export] int hp = 2;
        [Export] float maxSpeed = 50.0f;
        [Export] float separationWeight = 1.5f;
        [Export] float separationRadius = 55f;
        [Export] float maxTurnSpeed = 1.5f;
        [Export] float attackDistance = 200f;
        [Export] Sprite2D sprite;
        [Export] private AudioStreamPlayer takeHitSound;
        [Export] AnimationPlayer animation;
        private ShaderMaterial damageShader;
        private float damageFlashDuration = 0.4f;
        public Joueur joueur;
        private bool isAttaking = false;
        private bool hasGrabbedPlayer = false;
        private Vector2 velocity;
        private float attackSpeed = 800;
        private float maxAngleRad;
        public float attackDistancePosition;

        public override void _Ready()
        {
            attackDistancePosition = GetViewportRect().Size.Y - attackDistance;
            maxAngleRad = Mathf.DegToRad(40);
            damageShader = (ShaderMaterial)sprite.Material;
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
                MoveToTarget(toTarget.Normalized(), delta);
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

        private void MoveToTarget(Vector2 targetDirection, double delta)
        {
            velocity = ClampDirection(targetDirection) * maxSpeed;

            Vector2 separationForce = Vector2.Zero;

            var group = GetTree().GetNodesInGroup("zombie");

            // Séparation
            foreach (var node in group)
            {
                if (node == this || node is not Zombie otherZombie) continue;

                Vector2 otherZombieDirection = GlobalPosition - otherZombie.GlobalPosition;
                float distance = otherZombieDirection.Length();

                if (distance < separationRadius)
                {
                    float repulsionForce = (separationRadius - distance) / separationRadius;
                    separationForce += otherZombieDirection * repulsionForce * separationWeight;
                }
            }

            // Déplacement
            velocity = velocity + separationForce;
            Position += velocity * (float)delta;

            // Rotation lente
            RotateTowardTarget(velocity, delta);
        }

        private void RotateTowardTarget(Vector2 targetDirection, double delta)
        {
            Rotation = Mathf.LerpAngle(Rotation, targetDirection.Angle() - Mathf.Pi / 2f, maxTurnSpeed * (float)delta);
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
            if (area is IDamagableJoueur damagable)
            {
                if (damagable.canBeGrabbed)
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

        public void TakeDamage(int damage)
        {
            damageShader.SetShaderParameter("hit_amount", 1.0f);
            var tween = CreateTween();
            tween.TweenMethod(
            Callable.From<float>(value => damageShader.SetShaderParameter("hit_amount", value)),
            1.0f,
            0.0f,
            damageFlashDuration
        ).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            Shake();

            hp -= 1;
            if (hp <= 0)
            {
                CallDeferred(nameof(Die));
            }
            else
            {
                takeHitSound.Play();
            }
        }

        public void Die()
        {
            Monitorable = false;
            Monitoring = false;
            animation.Play("die");
            joueur.ScoreUpdateListener(100, GlobalPosition);
        }

        public async void Shake(float recoilStrength = 12f, float shakeStrength = 1.5f, float duration = 0.4f, int oscillations = 2)
        {
            Vector2 originalPosition = Position;
            var tween = CreateTween();
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);

            float step = duration / (oscillations + 2);

            // Recul initial (ex. vers le haut / gauche / selon ton besoin)
            tween.TweenProperty(this, "position", originalPosition + new Vector2(0, -recoilStrength), step * 0.6f);

            // Tremblement
            for (int i = 0; i < oscillations; i++)
            {
                var offset = new Vector2(
                    (float)GD.RandRange(-shakeStrength, shakeStrength),
                    (float)GD.RandRange(-shakeStrength, shakeStrength)
                );
                tween.TweenProperty(this, "position", originalPosition + offset, step);
            }

            // Retour doux
            tween.TweenProperty(this, "position", originalPosition, step * 1.8f);
        }

        public void OnScreenExited()
        {
            QueueFree();
        }
    }
}

