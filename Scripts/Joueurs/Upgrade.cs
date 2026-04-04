using System.Drawing;
using Godot;
using SpaceZombie.Events;
namespace SpaceZombie.Joueurs
{
    public partial class Upgrade : Area2D
    {
        [Export] private float vitesse = 60f;
        [Export] private float sideMoveDistance = 45f;
        [Export] private CollisionShape2D collisionShape;
        private static RandomNumberGenerator randomPosition = new RandomNumberGenerator();

        private Tween tween;

        public override void _Ready()
        {
            Move();
        }

        public void Initialize(float areaPlayWidth)
        {
            Vector2 size = ((RectangleShape2D)collisionShape.Shape).Size;
            float initialPostiionX = randomPosition.RandfRange(sideMoveDistance + size.X, areaPlayWidth - size.X);
            Position = new Vector2(initialPostiionX, -size.Y + 1);
        }

        public override void _PhysicsProcess(double delta)
        {
            GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y + (vitesse * (float)delta));
        }

        private void OnAreaEntered(Area2D area)
        {
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.ChooseUpgrade);
            Deactivate();
        }

        public void Deactivate()
        {
            Visible = false;
            tween.Stop();
            QueueFree();
        }

        public void MovedOutOfBound()
        {
            if (Visible)
            {
                Deactivate();
            }
        }

        private void Move()
        {
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.InOut);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetLoops();
            tween.TweenProperty(this, "position:x", GlobalPosition.X - sideMoveDistance, 2);
            tween.TweenProperty(this, "position:x", GlobalPosition.X, 2);
        }
    }

}