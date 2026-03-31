//MainCanva.cs
using Godot;
using SpaceZombie.Events;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class MainCanva : Control
    {
        [Export] private MainAera mainAera;
        public override void _Ready()
        {
            GameEvents.Instance.ShowEndScreen += ShowEndScreen;
            CallDeferred(nameof(DeferredInit));
        }

        private void DeferredInit()
        {
            mainAera.Initialiser();
        }

        private void ShowEndScreen()
        {
            PackedScene endScreenLoader = (PackedScene)ResourceLoader.Load($"res://Scenes/end_screen.tscn");
            EndScreen endScreen = endScreenLoader.Instantiate<EndScreen>();
            endScreen.score = mainAera.joueur.jState.Score;
            GetTree().Root.AddChild(endScreen);
            mainAera.QueueFree();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            GameEvents.Instance.ShowEndScreen -= ShowEndScreen;
        }
    }
}
