using Godot;
using SpaceZombie.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MenuUpgrade : PanelContainer
{
    [Signal]
    public delegate void UpgradeEventHandler(int option);
    [Export] Label selectionTip;
    [Export] ButtonUpgrade option1;
    [Export] ButtonUpgrade option2;
    [Export] string buttonNameEn;
    [Export] string buttonNameFr;

    private RandomNumberGenerator random = new RandomNumberGenerator();
    public override void _Ready()
    {
        option1.Upgrade += SelectUpgrade;
        option2.Upgrade += SelectUpgrade;
        string buttonName = TranslationServer.GetLocale() == "en" ? buttonNameEn : buttonNameFr;
        selectionTip.Text = string.Format(Tr("UPGRADE_INPUT"), buttonName);
    }

    private void SelectUpgrade(int option)
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Visible = false;
        EmitSignal(SignalName.Upgrade, option);
    }

    public void ChooseUpgrade(Joueur joueur)
    {
        ProcessMode = ProcessModeEnum.WhenPaused;
        int firstPick = random.RandiRange(0, 4);
        List<UpgradeOptions> options = Enum.GetValues<UpgradeOptions>().ToList();
        SetOption(option1, options[firstPick], joueur);

        int secondPick = random.RandiRange(0, 3);
        SetOption(option2, options.Except([options[firstPick]]).ToList()[secondPick], joueur);

        option1.GrabFocus();
        Visible = true;
    }

    private int GetPercentage(float percentage)
    {
        return (int)(percentage * 100);
    }

    private void SetOption(ButtonUpgrade button, UpgradeOptions option, Joueur joueur)
    {
        switch (option)
        {
            case UpgradeOptions.AddProjectile:
                button.SetUpgrage((int)option, addProjectileTitle, addProjectileTexture);
                break;
            case UpgradeOptions.Damage:
                button.SetUpgrage((int)option, Tr("UPGRADE_DAMAGE"), damageTexture);
                break;
            case UpgradeOptions.AttackSpeed:
                button.SetUpgrage((int)option, string.Format(Tr("UPGRADE_ATTACK_SPEED"), GetPercentage(joueur.canons.upgradeAttackSpeed)), attackSpeedTexture);
                break;
            case UpgradeOptions.Passthrough:
                button.SetUpgrage((int)option, Tr("UPGRADE_PASSTHROUGH"), passthroughTexture);
                break;
            case UpgradeOptions.MoveSpeed:
                button.SetUpgrage((int)option, string.Format(Tr("UPGRADE_MOVE_SPEED"), GetPercentage(joueur.upgradeMoveSpeed)), moveSpeedTexture);
                break;
        }
    }

    const string addProjectileTitle = "Projectile +1";
    private static readonly Texture2D addProjectileTexture = ResourceLoader.Load<Texture2D>("res://images/more_projectile.png");
    private static readonly Texture2D damageTexture = ResourceLoader.Load<Texture2D>("res://images/attack_damage_icon.png");
    private static readonly Texture2D attackSpeedTexture = ResourceLoader.Load<Texture2D>("res://images/attack_speed_icon.png");
    private static readonly Texture2D passthroughTexture = ResourceLoader.Load<Texture2D>("res://images/attack_piercing.png");
    private static readonly Texture2D moveSpeedTexture = ResourceLoader.Load<Texture2D>("res://images/move_speed_icon.png");
}
