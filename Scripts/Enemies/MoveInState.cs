using Godot;
using System;

public partial class MoveInState : State
{
    MoveInState() { name = move; }
    public float distancePosition = 0;

    public override void PhysicUpdate(double delta)
    {
        if (GlobalPosition.Y < distancePosition)
        {
            Vector2 direction = enemy.GetJoueurDirection();
            enemy.MoveToTarget(direction, delta);
            enemy.RotateTowardTarget(direction, delta);
        }
        else
        {
            enemy.ChangeState(attack);
        }
    }

}
