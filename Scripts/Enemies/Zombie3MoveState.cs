using Godot;
using SpaceZombie.Joueurs;
using System;

public partial class Zombie3MoveState : State
{
    Zombie3MoveState() { name = move; }

    public Vector2 targetPosition;
    public Vector2 targetDirection;
    [Export] Timer firstMoveTimer;
    private Action actionAterMove;

    public override void _Ready()
    {
        base._Ready();
        firstMoveTimer.Timeout += MoveToTarget;
        actionAterMove = ReadyToAttack;
    }

    public override void Enter()
    {
        if (actionAterMove == Attack)
        {
            SetTarget(new Vector2(enemy.joueur.GlobalPosition.X, enemy.GlobalPosition.Y));
        }
        else
        {
            SetTarget(new Vector2(GlobalPosition.X, GetViewportRect().Size.Y - GD.RandRange(425, 580)));
        }
    }
    public void SetTarget(Vector2 target)
    {
        targetPosition = target;
        targetDirection = (target - GlobalPosition).Normalized();
    }

    public override void PhysicUpdate(double delta)
    {
        if (enemy.GlobalPosition.DistanceSquaredTo(targetPosition) > 100)
        {
            enemy.MovePosition(targetDirection, delta);
        }
        else
        {
            actionAterMove();
        }
    }

    private void ReadyToAttack()
    {
        firstMoveTimer.Start();
        enemy.SetPhysicsProcess(false);
    }

    private void MoveToTarget()
    {
        actionAterMove = Attack;
        enemy.SetPhysicsProcess(true);
        Enter();
    }

    private void Attack()
    {
        enemy.ChangeState(attack);
    }
}
