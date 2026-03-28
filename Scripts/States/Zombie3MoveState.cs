using Godot;
using SpaceZombie.Joueurs;
using System;

public partial class Zombie3MoveState : State
{
    Zombie3MoveState() { name = move; }

    public Vector2 targetPosition;
    public Vector2 targetDirection;

    public override void Enter()
    {
        SetTarget(new Vector2(enemy.joueur.GlobalPosition.X, enemy.GlobalPosition.Y));
    }

    public void SetTarget(Vector2 target)
    {
        targetPosition = target;
        targetDirection = (target - GlobalPosition).Normalized();
    }

    public override void PhysicUpdate(double delta)
    {
        if (enemy.GlobalPosition.DistanceTo(targetPosition) > 10)
        {
            enemy.MovePosition(targetDirection, delta);
            //Vector2 direction = enemy.GetJoueurDirection();
            //enemy.MoveToTarget(direction, delta);
            //enemy.RotateTowardTarget(direction, delta);
        }
        else
        {
            enemy.ChangeState(attack);
        }
    }
}
