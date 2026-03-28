using System.Collections.Generic;
using Godot;
using SpaceZombie.Ammunitions;

public partial class Zombie2AttackState : State
{
    Zombie2AttackState() { name = attack; }
    [Export] int numberOfAttacks;
    [Export] int attackDamage;
    [Export] int attackSpeed;
    [Export] Timer attacksDelay;
    [Export] private PackedScene bulletLoader;
    private Queue<ProjectileObjet> projectiles;

    public override void _Ready()
    {
        base._Ready();
        attacksDelay.Timeout += Fire;
        InitializeProjectile();
    }

    public override void Enter()
    {
        attacksDelay.Start();
    }

    public override void Exit()
    {
        attacksDelay.Stop();
    }

    public override void Update(double delta)
    {
        if (attacksDelay.IsStopped() && projectiles.Count == numberOfAttacks)
        {
            attacksDelay.Start();
        }
    }

    public override void PhysicUpdate(double delta)
    {
        enemy.RotateTowardTarget(enemy.GetJoueurDirection(), delta);
    }

    private void InitializeProjectile()
    {
        projectiles = new Queue<ProjectileObjet>();
        for (int i = 0; i < numberOfAttacks; i++)
        {
            ProjectileObjet projectile = (ProjectileObjet)bulletLoader.Instantiate();
            projectile.Initialize(new Projectile(attackDamage, attackSpeed));
            projectile.OutOfBoundignal += ProjectileAvailable;
            projectiles.Enqueue(projectile);
            GetTree().CurrentScene.AddChild(projectile);
        }
    }
    private void ProjectileAvailable(ProjectileObjet projectile)
    {
        projectiles.Enqueue(projectile);
    }

    private void Fire()
    {
        projectiles.Dequeue().Fire(enemy.direction.Normalized(), GlobalPosition, 0);
        if (projectiles.Count == 0)
        {
            attacksDelay.Stop();
        }
    }
}
