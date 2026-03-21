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
    private int upgrade;
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
            EmitSignal(SignalName.Upgrade, upgrade);
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

    public void SetUpgrage(int upgrade, string title, Texture2D texture)
    {
        this.upgrade = upgrade;
        this.title.Text = title;
        icon.Texture = texture;

    }

    private void SetTitleAndIcon(String title, Texture2D texture)
    {
        this.title.Text = title;
        icon.Texture = texture;
    }
}
