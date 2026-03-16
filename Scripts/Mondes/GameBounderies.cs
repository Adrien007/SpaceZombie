using Godot;
using System;
using System.Reflection;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class GameBounderies : Area2D
    {
        CollisionShape2D collisionShape;

        public Vector2 screenSize;
        public override void _Ready()
        {
            collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
            GetViewport().SizeChanged += OnSizeChanged;
            OnSizeChanged();
        }

        private void OnSizeChanged()
        {
            screenSize = GetViewportRect().Size;
            GlobalPosition = screenSize / 2;
            ((RectangleShape2D)collisionShape.Shape).Size = screenSize;
            collisionShape.Position = Vector2.Zero;
        }
    }
}

