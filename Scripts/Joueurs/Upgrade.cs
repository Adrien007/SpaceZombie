using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;
using System;
namespace SpaceZombie.Joueurs
{
    public partial class Upgrade : Area2D
    {
        private RandomNumberGenerator randomPosition = new RandomNumberGenerator();
        private float vitesse = 60f;
        private float sideMoveDistance = 45f;
        private Tween tween;
        private float playAreaWidth;
        public override void _Ready()
        {
            AreaEntered += OnAreaEntered;
            SetProcess(false);
        }

        public override void _Process(double delta)
        {
            GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y + (vitesse * (float)delta));
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area is Joueur joueur)
            {
                Deactivate();
                GameEvents.Instance.EmitSignal(GameEvents.SignalName.ChooseUpgrade);
            }
        }

        public void Activate()
        {
            Visible = true;
            CallDeferred(Area2D.MethodName.SetMonitoring, true);
            GlobalPosition = new Vector2(randomPosition.RandfRange(sideMoveDistance, playAreaWidth), 0);
            SetProcess(true);
            move();
        }

        public void Deactivate()
        {
            CallDeferred(Area2D.MethodName.SetMonitoring, false);
            Visible = false;
            tween.Stop();
        }

        public void MovedOutOfBound()
        {
            if (Visible)
            {
                Deactivate();
            }
        }

        private void move()
        {
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.InOut);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetLoops();

            tween.TweenProperty(this, "position:x", GlobalPosition.X - sideMoveDistance, 2);
            tween.TweenProperty(this, "position:x", GlobalPosition.X, 2);
        }

        public void InitializePlayAreaSize(Vector2 size)
        {
            playAreaWidth = size.X;
        }
    }

}