using Godot;
using System;

public partial class Zombie3AttackState : State
{
    Zombie3AttackState() { name = attack; }
    [Export] Timer attackCooldown;

    public override void Enter()
    {
        attackCooldown.Start();
    }

    public override void Update(double delta)
    {
        if (attackCooldown.IsStopped())
        {
            enemy.ChangeState(move);
        }
    }
}
