
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Utilitaires;

public partial class Zombie1AttackState : State
{
    Zombie1AttackState() { name = attack; }

    private bool hasGrabbedPlayer = false;
    private float attackSpeed = 800;
    private bool isAttaking = false;

    public override void Enter()
    {
        enemy.animation.Play("prepare_attack");
    }
    public void Attack()
    {
        isAttaking = true;
    }

    public override void Exit()
    {
        enemy.animation.Stop();
    }

    public override void PhysicUpdate(double delta)
    {
        if (hasGrabbedPlayer)
        {
            ((Zombie1)enemy).StickToJoueur();
        }
        if (isAttaking)
        {
            enemy.MoveToDirection(attackSpeed, delta);
        }
        else
        {
            enemy.direction = enemy.GetJoueurDirection();
            enemy.RotateTowardTarget(enemy.direction, delta);
        }
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is IDamagable damagable)
        {
            if (!damagable.IsDodging)
            {
                hasGrabbedPlayer = true;
                isAttaking = false;
                CallDeferred(nameof(PlayerGrabbed));
            }
        }
    }

    private void PlayerGrabbed()
    {
        ((Zombie1)enemy).PlayerGrabbed();
    }
}
