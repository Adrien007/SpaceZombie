using System;
using Godot;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;

namespace SpaceZombie.Enemies
{
    public partial class BaseEnemy : Area2D, IDamagable
    {
        [Export] protected Sprite2D sprite;
        [Export] private AudioStreamPlayer takeHitSound;
        [Export] protected AnimationPlayer animation;
        [Export] protected int hp;
        [Export] protected float moveSpeed;
        [Export] protected float turnSpeed;
        [Export] private float separationRadius = 40f;
        private float separationWeight = 2f;
        private float damageFlashDuration = 0.4f;
        private ShaderMaterial damageShader;
        public Joueur joueur;
        public Action died;
        protected Vector2 velocity;

        public override void _Ready()
        {
            damageShader = (ShaderMaterial)sprite.Material;
        }

        public void Initialize(Vector2 position, Joueur joueur, Action died)
        {
            Position = position;
            this.joueur = joueur;
            this.died = died;
        }

        public void TakeDamage(int damage)
        {
            ShowHitShader();
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

        protected virtual void Die()
        {
            Monitorable = false;
            Monitoring = false;
            animation.Play("die");
            joueur.ScoreUpdateListener(100, GlobalPosition);
            died();
        }

        protected void MoveToTarget(Vector2 targetDirection, double delta)
        {
            velocity = targetDirection.Normalized() * moveSpeed;

            Vector2 separationForce = Vector2.Zero;

            var group = GetTree().GetNodesInGroup("zombie");

            // Séparation
            foreach (BaseEnemy other in group)
            {
                if (other == this) continue;

                Vector2 otherDirection = GlobalPosition - other.GlobalPosition;
                float distance = otherDirection.Length();

                if (distance < other.separationRadius)
                {
                    float repulsionForce = (other.separationRadius - distance) / separationRadius;
                    separationForce += otherDirection * repulsionForce * separationWeight;
                }
            }

            // Déplacement
            velocity = velocity + separationForce;
            Position += velocity * (float)delta;
        }

        protected void RotateTowardTarget(Vector2 targetDirection, double delta)
        {
            Rotation = Mathf.LerpAngle(Rotation, targetDirection.Angle() - Mathf.Pi / 2f, turnSpeed * (float)delta);
        }

        private void ShowHitShader()
        {
            damageShader.SetShaderParameter("hit_amount", 1.0f);
            var tween = CreateTween();
            tween.TweenMethod(
            Callable.From<float>(value => damageShader.SetShaderParameter("hit_amount", value)),
            1.0f,
            0.0f,
            damageFlashDuration
        ).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

        }

        public async void Shake(float recoilStrength = 10f, float shakeStrength = 1.5f, float duration = 0.4f, int oscillations = 2)
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
    }
}