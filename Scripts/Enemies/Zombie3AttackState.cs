using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using System;

public partial class Zombie3AttackState : State
{
    Zombie3AttackState() { name = attack; }
    [Export] LazerRay lazer;
    [Export] float followSpeed = 3;
    public override void _Ready()
    {
        base._Ready();

        lazer.fireEndedListener = FireEnded;
    }

    public override void Enter()
    {
        lazer.Aim();
    }

    public override void PhysicUpdate(double delta)
    {
        if (lazer.isAiming)
        {
            ((Zombie3)enemy).MoveSlowlyToward(new Vector2(enemy.joueur.GlobalPosition.X, enemy.GlobalPosition.Y), (float)delta);
        }
    }

    private void FireEnded()
    {
        ((Zombie3)enemy).StopMovingSlowlyToward();
        ((Zombie3)enemy).Cooldown();
    }

    public override void Exit()
    {
        lazer.Stop();
    }
}
