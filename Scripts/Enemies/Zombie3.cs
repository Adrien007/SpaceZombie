using Godot;
using System;

namespace SpaceZombie.Enemies
{
    public partial class Zombie3 : BaseEnemy
    {
        public override void _Ready()
        {
            base._Ready();
            ((Zombie3MoveState)currentState).SetTarget(new Vector2(Position.X, GetViewportRect().Size.Y - GD.RandRange(425, 580)));
        }
    }
}

