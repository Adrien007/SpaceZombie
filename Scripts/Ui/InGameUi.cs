//InGameUi.cs
using Godot;
using SpaceZombie.Events;

namespace SpaceZombie.Ui
{
    public partial class InGameUi : Control
    {
        [Export] Label viRestante;
        [Export] Label score;


        public override void _Ready()
        {
            GameEvents.Instance.PlayerScoreUpdated += UpdatePlayerScoreListener;
            GameEvents.Instance.PlayerHealthUpdated += UpdatePlayerHealthLeftListener;
        }

        private void UpdatePlayerScoreListener(int score)
        {
            this.score.Text = score.ToString();
        }
        private void UpdatePlayerHealthLeftListener(int playerHealth)
        {
            playerHealth -= 1;
            string txt = $"{playerHealth} x ";
            this.viRestante.Text = txt;
        }

    }
}