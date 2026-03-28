using System;
using Godot;
using SpaceZombie.Enemies;

public partial class State : Node2D
{
    public const string move = "Move";
    public const string attack = "Attack";
    public string name;
    public BaseEnemy enemy;
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update(double delta) { }
    public virtual void PhysicUpdate(double delta) { }
}