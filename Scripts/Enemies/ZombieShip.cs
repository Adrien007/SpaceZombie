using System.Collections;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;

public partial class ZombieShip : BaseEnemy
{
    [Export] int numberOfAttacks;
    [Export] int attackDamage;
    [Export] int attackSpeed;
    [Export] Timer attacksDelay;
    [Export] Node2D canon;
    [Export] private PackedScene bulletLoader;
    private bool isInPosition = false;
    public float attackDistancePosition;
    private float offsetRadius = 30f;
    private bool isAttaking = false;
    private Queue<ProjectileObjet> projectiles;

    public override void _Ready()
    {
        base._Ready();
        attacksDelay.Timeout += Fire;
        InitializeProjectile();
        attackDistancePosition = GetViewportRect().Size.Y - GD.RandRange(350, 580);
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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (GlobalPosition.Y < attackDistancePosition)
        {
            MoveToTarget(joueur.GlobalPosition - GlobalPosition, delta);
        }
        else
        {
            if (attacksDelay.IsStopped())
            {
                if (projectiles.Count == numberOfAttacks)
                {
                    attacksDelay.Start();
                }
            }
            else if (projectiles.Count == 0)
            {
                attacksDelay.Stop();
            }
            velocity = joueur.GlobalPosition - GlobalPosition;
        }
        RotateTowardTarget(velocity, delta);
    }

    private void Fire()
    {
        projectiles.Dequeue().Fire(velocity.Normalized(), canon.GlobalPosition, 0);
    }

    private void ProjectileAvailable(ProjectileObjet projectile)
    {
        projectiles.Enqueue(projectile);
    }

    protected override void Die()
    {
        base.Die();
        attacksDelay.Stop();
        SetProcess(false);
    }
}
