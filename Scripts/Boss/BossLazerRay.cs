using Godot;
using System;
namespace SpaceZombie.Boss
{
    public partial class BossLazerRay : Node2D
    {
        public AnimationPlayer animation;

        private RayCast2D rayCast;
        
        private Line2D laserMain;
        
        private Line2D laserGlow;
        
        private Line2D laserCore;
        
        private bool isFiring = false;

        public Action lazerCollideListener;

        public override void _Ready()
        {
            animation = GetNode<AnimationPlayer>("AnimationPlayer");
            rayCast = GetNode<RayCast2D>("RayCast2D");
            SetPhysicsProcess(false);
            
            laserMain = GetNode<Line2D>("LaserMain");
            laserGlow = GetNode<Line2D>("LaserGlow");
            laserCore = GetNode<Line2D>("LaserCore");
            
            rayCast.TargetPosition = new Vector2(0, 1000);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (laserMain == null || laserGlow == null || laserCore == null || !isFiring || !rayCast.Enabled)
                return;
                
                
            rayCast.ForceRaycastUpdate();

            Vector2 start = Vector2.Zero;

            Vector2 direction = rayCast.TargetPosition.Normalized();

            float length = 1000f;

            if (rayCast.IsColliding())
            {
                Vector2 collisionPoint = ToLocal(rayCast.GetCollisionPoint());
                length = collisionPoint.Length();
                
                lazerCollideListener?.Invoke();
            }
            
            
            if (length < 5f)
            {
                laserMain.Visible = false;
                laserGlow.Visible = false;
                laserCore.Visible = false;
                return;
            }
            laserMain.Visible = true;
            laserGlow.Visible = true;
            laserCore.Visible = true;
            

            Vector2 end = direction * length;
            
            float pulse = Mathf.Sin((float)Time.GetTicksMsec() * 0.01f);
            
            laserMain.Width = 20 + pulse * 3;
            laserGlow.Width = 30 + pulse * 5;
            laserCore.Width = 10 + pulse * 2;

            Vector2[] points = new Vector2[] { start, end };

            laserMain.Points = points;
            laserGlow.Points = points;
            laserCore.Points = points;
        }

        public async void AnimationStarted()
        {
            rayCast.Enabled = true;
            SetPhysicsProcess(true);
            rayCast.ForceRaycastUpdate();
            
            isFiring = true;
        }

        public async void AnimationStopped()
        {
            isFiring = false;
            rayCast.TargetPosition = Vector2.Zero;
            rayCast.ForceRaycastUpdate();
            SetPhysicsProcess(false);
            rayCast.Enabled = false;
            
            laserMain.Visible = false;
            laserGlow.Visible = false;
            laserCore.Visible = false;
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
            if (!rayCast.Enabled) return;
            
            animation.Stop();
            AnimationStopped();
        }
    }
}
