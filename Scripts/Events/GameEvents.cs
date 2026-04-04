using Godot;

namespace SpaceZombie.Events
{
    public partial class GameEvents : Node
    {
        public static GameEvents Instance { get; private set; }

        public override void _EnterTree()
        {
            if (Instance != null && Instance != this)
            {
                GD.PrintErr("Multiple GameEvents detected! Removing duplicate.");
                QueueFree();
                return;
            }
            Instance ??= this;
        }


        [Signal]
        public delegate void UpdateScoreEventHandler(int score, Vector2 globalPosition);

        [Signal]
        public delegate void EnemyDiedEventHandler();

        [Signal]
        public delegate void ChooseUpgradeEventHandler();

        [Signal]
        public delegate void EndLevelEventHandler(string level);

        [Signal]
        public delegate void PlayerScoreUpdatedEventHandler(int playerScore);

        [Signal]
        public delegate void PlayerHealthUpdatedEventHandler(int playerHealth);

        [Signal]
        public delegate void ShowEndScreenEventHandler();
    }
}

