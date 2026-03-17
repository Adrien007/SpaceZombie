//ProjectileObjet.cs
using Godot;
using SpaceZombie.Events;
using SpaceZombie.Utilitaires.Layers;

namespace SpaceZombie.Ammunitions
{
    public partial class ProjectileObjet : Node2D, IResetEtatObserver
    {
        public delegate void HitSignalEventHandler(ProjectileObjet projectileObj);
        public event HitSignalEventHandler OutOfBoundignal;
        [Export] private Area2D area;
        private Projectile projectile;
        private Vector2 directionXY;

        public Projectile Projectile { get => projectile; }

        public override void _Ready()
        {
            base._Ready();
            Disable();
        }
        public void Initialize(Projectile projectile,
                                IResetEtatNotifier resetEtatNotifier)
        {
            area.AreaExited += OnAreaExited;
            this.projectile = projectile;
            resetEtatNotifier.Register(this);
        }

        //private const float CORRECTION_ANGLE = Mathf.Pi * 0.5f;
        public void Fire(Vector2 directionXY, Vector2 globalPosition, float globalRotation)
        {
            this.directionXY = directionXY;
            this.GlobalPosition = globalPosition;
            this.GlobalRotation = globalRotation;// + CORRECTION_ANGLE;
            Enable();
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Visible)
            {
                GlobalPosition += directionXY * projectile.Vitesse * (float)delta;
            }
        }

        private void OnAreaExited(Area2D aera2D)
        {
            //GD.Print("AreaExited: " + aera2D.CollisionLayer);
            const uint layer1 = 1u << 0; // Layer 1 is the 1st bit (index 0)
            if ((aera2D.CollisionLayer & layer1) != 0)
            {
                //GD.Print("OnAreaExited + " + this.Name);
                //GD.Print("AreaExited: " + aera2D.GetType() + "  " + aera2D.CollisionLayer);
                // Defer the call to Disable() to avoid issues during signal processing
                CallDeferred(nameof(Disable));
                OutOfBoundignal.Invoke(this);
            }
        }

        public void Disable()
        {
            //area.Monitoring = false;
            area.CallDeferred(Area2D.MethodName.SetMonitorable, false);
            Visible = false;
        }
        private void Enable()
        {
            Visible = true;
            area.CallDeferred(Area2D.MethodName.SetMonitorable, true);
            //area.Monitoring = true;
        }

        public void OnResetToInitaialState()
        {
            if (!Visible)//Si est un projectile NON tire; Pas besoin de le reset.
                return;
            Disable();//Le fait de Disable, on disable le area.Monitoring, ce qui semble appeler OnAreaExited.
        }

        public void StartTimerState() { /*This class has no timer*/ }
    }
}
