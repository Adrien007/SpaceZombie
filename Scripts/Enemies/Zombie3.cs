using Godot;
using System;

namespace SpaceZombie.Enemies
{
    public partial class Zombie3 : BaseEnemy
    {
        [Export] private Node2D smokeVfx;
        private Vector2 velocity = Vector2.Zero;
        private float acceleration = 100.0f;
        public float maxSpeed = 75.0f;
        private Vector2 startPosition;

        public override void _Ready()
        {
            base._Ready();
            this.score = 568;
            startPosition = this.Position;
        }

        public void MoveSlowlyToward(Vector2 targetPosition, float delta)
        {
            // Direction toward the target
            Vector2 directionToTarget = (targetPosition - GlobalPosition).Normalized();

            // Desired velocity = constant speed in the current direction to target
            Vector2 targetVelocity = directionToTarget * maxSpeed;

            // Smoothly change current velocity toward the new target velocity
            // This creates the "slow reaction to direction changes"
            velocity = velocity.MoveToward(targetVelocity, acceleration * delta);

            // Apply the movement
            GlobalPosition += velocity * delta;
        }

        public void StopMovingSlowlyToward()
        {
            velocity = Vector2.Zero;
        }

        public void Cooldown()
        {
            animation.Play("cooldown");
        }

        public void CooldownEnded()
        {
            ChangeState(State.move);
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            Vector2 screenSize = GetViewportRect().Size;
            // Margin outside screen before correction
            const float margin = 200f;
            if (GlobalPosition.X < -margin || GlobalPosition.X > screenSize.X + margin ||
                GlobalPosition.Y < -margin || GlobalPosition.Y > screenSize.Y + margin)
            {
                // Option A: Clamp back inside
                GlobalPosition = new Vector2(
                    Mathf.Clamp(GlobalPosition.X, 0, screenSize.X),
                    Mathf.Clamp(GlobalPosition.Y, 0, screenSize.Y)
                );
                ChangeState(State.attack);

                // Optional debug
                GD.Print($"Enemy corrected: {Name}");
            }
        }
    }
}

