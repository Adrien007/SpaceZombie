using Godot;
using SpaceZombie.Utilitaires;
using System;

public partial class Damagable : Area2D, IDamagable
{
    [Signal] public delegate void take_damageEventHandler(int damage);

    public void TakeDamage(int damage)
    {
        EmitSignal(SignalName.take_damage, damage);
    }
}
