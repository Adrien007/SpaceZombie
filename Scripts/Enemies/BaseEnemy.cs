using System;
using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;

namespace SpaceZombie.Enemies
{
    public partial class BaseEnemy : Area2D, IDamagable
    {
        [Export] protected Sprite2D sprite;
        [Export] private AudioStreamPlayer takeHitSound;
        [Export] public AnimationPlayer animation;
        [Export] protected int hp;
        [Export] protected float moveSpeed;
        [Export] protected float turnSpeed;
        [Export] private float separationRadius = 40f;
        [Export] private State[] states;
        [Export] protected State currentState;
        private float separationWeight = 60f;
        private float damageFlashDuration = 0.4f;
        private ShaderMaterial damageShader;
        public Joueur joueur;
        public Vector2 direction;

        public override void _Ready()
        {
            damageShader = (ShaderMaterial)sprite.Material;
            foreach (State state in states)
            {
                state.enemy = this;
            }
            currentState.Enter();
        }

        public void Initialize(Vector2 position, Joueur joueur)
        {
            Position = position;
            this.joueur = joueur;
        }

        public override void _Process(double delta)
        {
            currentState.Update(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            currentState.PhysicUpdate(delta);
        }

        public void ChangeState(string newStateName)
        {
            currentState.Exit();
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].name == newStateName)
                {
                    currentState = states[i];
                    currentState.Enter();
                    return;
                }
            }
            throw new($"State {newStateName} not found !"); ;
        }

        public void TakeDamage(int damage)
        {
            ShowHitShader();
            if (direction.Length() < 700) Shake();

            hp -= damage;
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
            currentState.Exit();
            animation.Play("die");
            joueur.ScoreUpdateListener(100, GlobalPosition);
        }

        public void Remove()
        {
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.EnemyDied);
            SetProcess(false);
            SetPhysicsProcess(false);
            QueueFree();
        }

        public virtual Vector2 GetJoueurDirection()
        {
            return joueur.GlobalPosition - GlobalPosition;
        }

        public void MoveToTarget(Vector2 targetDirection, double delta)
        {
            direction = targetDirection.Normalized() * moveSpeed;

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
            direction = direction + separationForce;
            Position += direction * (float)delta;
        }

        public void MoveToDirection(float speed, double delta)
        {
            direction = direction.Normalized() * speed;
            Position += direction * (float)delta;
        }

        public void MovePosition(Vector2 direction, double delta)
        {
            GlobalPosition += direction * moveSpeed * (float)delta;
        }

        public void RotateTowardTarget(Vector2 targetDirection, double delta)
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