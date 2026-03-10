//ProchainNiveauUi.cs
using Godot;

namespace SpaceZombie.Ui
{
    public partial class ProchainNiveauUi : Control
    {
        [Export] ColorRect p;
        [Export] Label label;

        public Timer timer;

        public override void _Ready()
        {
            timer = new Timer();
            timer.OneShot = true;
            timer.WaitTime = 3f;
            this.AddChild(timer);
        }

        public void StartTimer() { timer.Start(); }
        public double TimeLeft() { return timer.TimeLeft; }

        public void UpdateLabelTexte(string txt)
        {

            label.Text = " : " + txt;
        }
    }
}