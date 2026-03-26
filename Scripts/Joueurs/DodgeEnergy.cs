using Godot;
using System;

public partial class DodgeEnergy : Sprite2D
{

    [Export] private AnimationPlayer animation;
    public void Use()
    {
        animation.Play("use");
    }

    public void Restore()
    {
        animation.Play("restore");
    }
}
