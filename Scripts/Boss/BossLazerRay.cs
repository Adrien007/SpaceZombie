using Godot;
using System;
namespace SpaceZombie.Boss
{
    public partial class BossLazerRay : Node2D
    {
        public AnimationPlayer animation;

        private RayCast2D rayCast;

        public Action lazerCollideListener;

        public override void _Ready()
        {
            animation = GetNode<AnimationPlayer>("AnimationPlayer");
            rayCast = GetNode<RayCast2D>("RayCast2D");
            SetPhysicsProcess(false);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (rayCast.IsColliding())
            {
                lazerCollideListener();
            }
        }

        public async void AnimationStarted()
        {
            rayCast.Enabled = true;
            SetPhysicsProcess(true);
            rayCast.TargetPosition = Vector2.Zero;
            rayCast.ForceRaycastUpdate();
        }

        public async void AnimationStopped()
        {
            rayCast.TargetPosition = Vector2.Zero;
            rayCast.ForceRaycastUpdate();
            SetPhysicsProcess(false);
            rayCast.Enabled = false;
        }

        public void StartFire()
        {
            if (animation.IsAnimationActive())
            {
                StopFire();
            }
            AnimationStarted();
            animation.Play("Fire");
        }

        public void StopFire()
        {
            animation.Stop();
        }
    }
}

