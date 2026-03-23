//ProjectileObjet.cs
using Godot;
using SpaceZombie.Utilitaires;

namespace SpaceZombie.Ammunitions
{
    public partial class ProjectileObjet : Area2D
    {
        public delegate void HitSignalEventHandler(ProjectileObjet projectileObj);
        public event HitSignalEventHandler OutOfBoundignal;
        private Projectile projectile;
        private Vector2 directionXY;
        private int traverse = 0;

        public Projectile Projectile { get => projectile; }

        public override void _Ready()
        {
            Visible = false;
        }
        public void Initialize(Projectile projectile)
        {
            AreaEntered += OnAreaEntered;
            this.projectile = projectile;
        }

        //private const float CORRECTION_ANGLE = Mathf.Pi * 0.5f;
        public void Fire(Vector2 directionXY, Vector2 globalPosition, float globalRotation)
        {
            this.directionXY = directionXY;
            this.GlobalPosition = globalPosition;
            this.GlobalRotation = globalRotation;// + CORRECTION_ANGLE;
            Enable();
        }

        public override void _Process(double delta)
        {
            // VisibleOnScreenEnabler2D Disable le Process quand le node est à l'extérieur de l'écran.
            GlobalPosition += directionXY * projectile.Vitesse * (float)delta;
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is IDamagable damagableNode)
            {
                //GD.Print($"Travere Projectile : {projectile.Traverse}, Traverse : {traverse}");
                //GD.Print($"Projectile {GetInstanceId()}, Hit : {area.Name} {area.GetInstanceId()}");
                damagableNode.TakeDamage(projectile.Damage);
                if (projectile.Traverse <= traverse)
                {
                    traverse = 0;
                    CallDeferred(nameof(Disable));
                }
                else
                {
                    traverse += 1;
                }
            }
        }

        public void Disable()
        {
            if (Visible)
            {
                Visible = false;
                CallDeferred(Area2D.MethodName.SetMonitoring, false);
            }
        }
        private void Enable()
        {
            if (!Visible)
            {
                traverse = 0;
                Visible = true;
                CallDeferred(Area2D.MethodName.SetMonitoring, true);
            }
        }

        public void MovedOutOfBound()
        {
            Disable();
            OutOfBoundignal.Invoke(this);
        }
    }
}
