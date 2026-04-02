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
            GD.Print("Cooldown");
            animation.Play("cooldown");
        }

        public void CooldownEnded()
        {
            GD.Print("CooldownEnded");
            ChangeState(State.move);
        }
    }
}

