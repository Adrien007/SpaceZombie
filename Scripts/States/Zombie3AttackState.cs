using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using System;

public partial class Zombie3AttackState : State
{
    Zombie3AttackState() { name = attack; }
    [Export] LazerRay lazer;
    [Export] Timer firstAttackTimer;
    [Export] Timer attackCooldown;
    [Export] float followSpeed = 3;
    private bool isFirstAttack = true;
    public override void _Ready()
    {
        base._Ready();
        attackCooldown.Timeout += CooldownEnded;
        firstAttackTimer.Timeout += FirstAttack;
        lazer.fireEndedListener = FireEnded;
    }

    public override void Enter()
    {
        if (isFirstAttack)
        {
            firstAttackTimer.Start();
        }
        else
        {
            lazer.Aim();
        }
    }

    public override void PhysicUpdate(double delta)
    {
        if (lazer.isAiming)
        {
            ((Zombie3)enemy).MoveSlowlyToward(new Vector2(enemy.joueur.GlobalPosition.X, enemy.GlobalPosition.Y), (float)delta);
        }
    }

    private void CooldownEnded()
    {
        enemy.ChangeState(move);
    }

    private void FireEnded()
    {
        ((Zombie3)enemy).StopMovingSlowlyToward();
        attackCooldown.Start();
    }

    private void FirstAttack()
    {
        isFirstAttack = false;
        enemy.ChangeState(move);
    }

    public override void Exit()
    {
        lazer.Stop();
    }
}
