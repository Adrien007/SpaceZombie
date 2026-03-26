using Godot;
using System;

public partial class EndScreen : ColorRect
{
    [Export] private Label labelScore;
    [Export] private Label labelNextTip;
    [Export] private AnimationPlayer animation;
    [Export] string buttonNameEn;
    [Export] string buttonNameFr;
    public int score;
    public override void _Ready()
    {
        labelScore.Text = $"Score : {score} points";
        string buttonName = TranslationServer.GetLocale() == "en" ? buttonNameEn : buttonNameFr;
        labelNextTip.Text = string.Format(Tr("END_BUTTON"), buttonName);
        animation.Play("fade_in");
        animation.Seek(0.0f, true);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("select"))
        {
            GetTree().ChangeSceneToFile("res://Scenes/title_screen.tscn");
            QueueFree();
        }
    }

    public void ReadyToReturnToTitleScreen()
    {
        GetTree().Root.SetProcess(false);
        GetTree().Root.SetPhysicsProcess(false);
        ProcessMode = ProcessModeEnum.Always;
    }
}
