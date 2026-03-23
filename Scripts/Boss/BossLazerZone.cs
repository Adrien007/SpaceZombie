using Godot;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;
using System;
using System.Collections.Generic;

namespace SpaceZombie.Boss
{
    public partial class BossLazerZone : Panel
    {
        private Area2D area;
        private CollisionShape2D collision;
        public AnimationPlayer animation;
        public int damage = 2;

        public override void _Ready()
        {
            area = GetNode<Area2D>("Area2D");
            collision = (CollisionShape2D)FindChild("CollisionShape2D");
            animation = GetNode<AnimationPlayer>("AnimationPlayer");
            area.AreaEntered += OnAreaEntered;
            DisableCollision();
        }

        public void Fire(Vector2 size, float positionX)
        {
            Size = size;
            GlobalPosition = new Vector2(positionX, 0);
            animation.Play("Fire");
        }

        public async void EnableCollision()
        {
            collision.Shape = (RectangleShape2D)collision.Shape.Duplicate();
            (collision.Shape as RectangleShape2D).Size = Size;
            area.Position = Size / 2;
            await ToSignal(GetTree(), "physics_frame");
        }

        public async void DisableCollision()
        {
            Vector2 screenSize = GetViewportRect().Size;
            area.Position = screenSize + Size;
        }

        public void OnAreaEntered(Area2D area)
        {
            if (area is IDamagable damagableNode)
            {
                damagableNode.TakeDamage(2);
            }
        }

        public void StopAnimation()
        {
            if (animation.IsAnimationActive())
            {
                animation.Stop();
            }
        }
    }

}
