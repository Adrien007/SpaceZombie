using System.Collections;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;

public partial class Zombie2 : BaseEnemy
{
    public override void _Ready()
    {
        base._Ready();
        ((MoveInState)currentState).distancePosition = GetViewportRect().Size.Y - GD.RandRange(425, 580);
    }

    protected override void Die()
    {
        base.Die();
        SetProcess(false);
    }
}
