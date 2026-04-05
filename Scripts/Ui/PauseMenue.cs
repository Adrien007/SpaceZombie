//PauseMenue.cs
using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceZombie.Ui
{
    public partial class PauseMenue : PanelContainer
    {
        [Signal]
        public delegate void UpgradeEventHandler(int option);
        [Export] Button resume;
        [Export] Button quit;
        private bool actionPressAlreadyPress = false;

        public override void _Ready()
        {
            resume.Pressed += ResumePressed;
            quit.Pressed += QuitPressed;
        }

        private void ResumePressed()
        {
            ProcessMode = ProcessModeEnum.Inherit;
            this.Visible = false;
            actionPressAlreadyPress = false;
            GetTree().Paused = false;
        }
        private void QuitPressed()
        {
            ProcessMode = ProcessModeEnum.Inherit;
            this.Visible = false;
            actionPressAlreadyPress = false;
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.ShowEndScreen);
            GetTree().Paused = false;
        }
        private void ActionPressed()
        {
            if (actionPressAlreadyPress)
                return;
            actionPressAlreadyPress = true;
            ProcessMode = ProcessModeEnum.WhenPaused;
            GetTree().Paused = true;
            resume.GrabFocus();
            this.Visible = true;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("pause_menue"))
            {
                ActionPressed();
            }
        }
    }
}
