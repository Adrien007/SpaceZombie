using Godot;
using SpaceZombie.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MenuUpgrade : PanelContainer
{
    [Signal]
    public delegate void UpgradeEventHandler(int option);
    [Export] ButtonUpgrade option1;
    [Export] ButtonUpgrade option2;

    private RandomNumberGenerator random = new RandomNumberGenerator();
    public override void _Ready()
    {
        option1.Upgrade += SelectUpgrade;
        option2.Upgrade += SelectUpgrade;
    }

    private void SelectUpgrade(int option)
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Visible = false;
        EmitSignal(SignalName.Upgrade, option);
    }

    public void ChooseUpgrade()
    {
        ProcessMode = ProcessModeEnum.WhenPaused;
        int firstPick = random.RandiRange(0, 4);
        List<UpgradeOptions> options = Enum.GetValues<UpgradeOptions>().ToList();
        option1.SetUpgrage(options[firstPick]);

        int secondPick = random.RandiRange(0, 3);
        option2.SetUpgrage(options.Except([option1.upgrade]).ToList()[secondPick]);

        option1.GrabFocus();
        Visible = true;

        //GD.Print($"option1 : {(int)option1.upgrade}, option2 : {(int)option2.upgrade}");
    }
}