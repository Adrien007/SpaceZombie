//FloatingText.cs
using Godot;

namespace SpaceZombie.Ui
{
    public partial class FloatingText : Node2D
    {
        [Export] public float FloatDistance = 40f;
        [Export] public float Duration = 1.0f;

        [Export] private Label label;

        public override void _Ready()
        {

        }

        public void ShowText(string text)
        {
            label.Text = text;

            label.Modulate = new Color(1, 1, 1, 1);
            var tween = CreateTween();

            tween.SetParallel(true);

            // Move up
            tween.TweenProperty(this, "position:y", Position.Y - FloatDistance, Duration);
            // Fade out
            tween.TweenProperty(label, "modulate:a", 0.0f, Duration);
            // Pop when appearing
            Scale = new Vector2(0.5f, 0.5f);
            tween.TweenProperty(this, "scale", Vector2.One, 0.15f);

            tween.Finished += OnTweenFinished;//QueueFree;
        }

        private void OnTweenFinished()
        {
            FloatingTextManager.Instance.ReturnToPool(this);
        }
    }
}