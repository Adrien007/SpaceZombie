using System;
using Godot;
using SpaceZombie.Utilitaires;

public partial class BossLazerZone : Control
{
    [Export] ColorRect zone1;
    [Export] ColorRect zone2;
    [Export] Area2D zone1Area;
    [Export] Area2D zone2Area;
    [Export] CollisionShape2D zone1ColShape;
    [Export] CollisionShape2D zone2ColShape;
    [Export] Timer attackDelayTimer;
    [Export] AudioStreamPlayer prepareAttackSound;
    [Export] AudioStreamPlayer attackSound;
    [Export] AnimationPlayer animation;
    public float attackZoneMargin = 80f;
    public int damage = 1;
    private Tween fireTween;
    private ShaderMaterial zoneMaterial;
    public Action attackEndedListener;
    public override void _Ready()
    {
        attackDelayTimer.Timeout += Attack;
    }

    public void Fire(float freeZoneWidth, float attackSpeed)
    {
        fireTween?.Kill();
        Vector2 screenSize = GetViewportRect().Size;
        float freeZonePosition = (float)GD.RandRange(attackZoneMargin, screenSize.X - attackZoneMargin - freeZoneWidth);
        
        zone1.Position = new Vector2(freeZonePosition - zone1.Size.X - attackZoneMargin, 0);
        zone1Area.Position = new Vector2(zone1.Size.X / 2, zone1.Size.Y / 2); // center collision
        ((RectangleShape2D)zone1ColShape.Shape).Size = zone1.Size; // match collision

        zone2.Position = new Vector2(freeZonePosition + freeZoneWidth, 0);
        zone2Area.Position = new Vector2(zone2.Size.X / 2, zone2.Size.Y / 2);
        ((RectangleShape2D)zone2ColShape.Shape).Size = zone2.Size;

        animation.Play("prepare_attack", attackSpeed);
    }

    public void StartAttackDelay()
    {
        attackDelayTimer.Start();
    }

    private async void Attack()
    {
        animation.Play("attack");
    }

    private void AttackEnded()
    {
        attackEndedListener();
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is IDamagable damagable && !damagable.IsDodging)
        {
            damagable.TakeDamage(damage);
        }
    }

    public void Stop()
    {
        zone1Area.Monitoring = false;
        zone2Area.Monitoring = false;
        zone1.Visible = false;
        zone2.Visible = false;
        fireTween?.Kill();
    }
}
