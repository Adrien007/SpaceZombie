using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using System;
using System.Reflection.Metadata;

public partial class ButtonUpgrade : PanelContainer
{
    [Signal]
    public delegate void UpgradeEventHandler(int option);
    [Export] private StyleBoxFlat noFocusStyle;
    [Export] private StyleBoxFlat focusStyle;
    [Export] private Label title;
    [Export] private TextureRect icon;
    public UpgradeOptions upgrade;
    public override void _Ready()
    {
        FocusEntered += OnFocusEntered;
        FocusExited += OnFocusExited;
        FocusMode = FocusModeEnum.All;
    }

    public override void _Process(double delta)
    {
        if (HasFocus() && Input.IsActionJustReleased("select"))
        {
            //GD.Print($"SelectUpgrade : {upgrade}");
            EmitSignal(SignalName.Upgrade, (int)upgrade);
        }
    }

    private void OnFocusEntered()
    {

        AddThemeStyleboxOverride("panel", focusStyle);
        title.AddThemeColorOverride("font_color", Colors.White);
        icon.UseParentMaterial = false;
    }

    private void OnFocusExited()
    {
        AddThemeStyleboxOverride("panel", noFocusStyle);
        title.AddThemeColorOverride("font_color", Colors.Black);
        icon.UseParentMaterial = true;
    }

    public void SetUpgrage(UpgradeOptions upgrade)
    {
        this.upgrade = upgrade;
        switch (upgrade)
        {
            case UpgradeOptions.AddProjectile: SetTitleAndIcon(addProjectileTitle, addProjectileTexture); break;
            case UpgradeOptions.Damage: SetTitleAndIcon(damageTitle, damageTexture); break;
            case UpgradeOptions.AttackSpeed: SetTitleAndIcon(attackSpeedTitle, attackSpeedTexture); break;
            case UpgradeOptions.Passthrough: SetTitleAndIcon(passthroughTitle, passthroughTexture); break;
            case UpgradeOptions.MoveSpeed: SetTitleAndIcon(moveSpeedTitle, moveSpeedTexture); break;
        }
    }

    private void SetTitleAndIcon(String title, Texture2D texture)
    {
        this.title.Text = title;
        icon.Texture = texture;
    }

    const String addProjectileTitle = "Projectile +1";
    const String damageTitle = "Damage +1";
    const String attackSpeedTitle = "Attack Speed +10%";
    const String passthroughTitle = "Passthrough +1";
    const String moveSpeedTitle = "Move Speed +10%";
    private static readonly Texture2D addProjectileTexture = ResourceLoader.Load<Texture2D>("res://images/more_projectile.png");
    private static readonly Texture2D damageTexture = ResourceLoader.Load<Texture2D>("res://images/attack_damage_icon.png");
    private static readonly Texture2D attackSpeedTexture = ResourceLoader.Load<Texture2D>("res://images/attack_speed_icon.png");
    private static readonly Texture2D passthroughTexture = ResourceLoader.Load<Texture2D>("res://images/attack_piercing.png");
    private static readonly Texture2D moveSpeedTexture = ResourceLoader.Load<Texture2D>("res://images/move_speed_icon.png");
}