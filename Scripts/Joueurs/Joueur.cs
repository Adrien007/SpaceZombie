using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
using SpaceZombie.Mondes.Utilitaires;
using SpaceZombie.Utilitaires.Layers;

namespace SpaceZombie.Joueurs
{
    /// <summary>
    /// Represents a player in the game, handling movement and shooting mechanics.
    /// </summary>
    public partial class Joueur : Node2D, IInitialisationSize, IInitialisationPosition, IResetEtatObserver
    {
        [Export] private Control enfant;
        [Export] public float vitesse = 200f;
        [Export] private Area2D area;
        [Export] private CannonJoueur cannons;

		public JoueurEtat jState;
		private Vector2 playAeraSize;
		private Vector2 playAeraPosition;
		private int longueurX;
		private Vector2 nouvellePosition;
		private float directionX = 0;

        public override void _Ready()
        {
            base._Ready();
            longueurX = (int)enfant.Size.X;
            playAeraSize = enfant.Size;
            playAeraPosition = Position;

            area.AreaEntered += OnAreaEntered;
            GameEvents.Instance.LevelUp += LevelUpCannon;
            GameEvents.Instance.EnemyDied += ScoreUpdateListener;
        }

		public override void _PhysicsProcess(double delta)
		{
			// Get movement direction from input
			directionX = 0f;
			if (Input.IsActionPressed("move_left"))
			{
				directionX = -1f; // Move left
			}
			else if (Input.IsActionPressed("move_right"))
			{
				directionX = 1f; // Move right
			}

			// Move the object along the X-axis based on input
			Position += new Vector2(directionX * vitesse * (float)delta, 0);

			// Clamp the position within the play area
			nouvellePosition.X = Mathf.Clamp(Position.X, playAeraPosition.X, playAeraPosition.X + playAeraSize.X - longueurX);
			nouvellePosition.Y = Position.Y;

			Position = nouvellePosition;

            // Check if spacebar is pressed and reload timer is not active
            if (Input.IsActionPressed("shot_fire"))
            {
                cannons.Fire();
            }
        }

        private void OnAreaEntered(Area2D area)
        {
            if (area.GetParent() is ProjectileObjet projectile)
            {
                if (!jState.IsDead && !jState.IsInvicible)
                {
                    jState.IsInvicible = true;
                    jState.InvincibilityTimer.Start();
                    jState.Hp = RetirerHp(jState.Hp, projectile.Projectile.Damage);
                    UpdateScore(-1000);
                    GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerHealthUpdated, jState.Hp);
                    if (jState.Hp <= 0)
                    {
                        jState.IsDead = true;
                        jState.DeadSoundPlayed = true;
                        GD.Print("[SoundSystemJoueur] Play 'player Die' sound.");
                        CallDeferred(nameof(Disable));
                        GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerDied);
                    }
                    else
                    {
                        GD.Print("[SoundSystemJoueur] Play 'player hit' sound.");
                    }
                }
                projectile.Disable();
            }
        }

        private static int RetirerHp(int hp, int hitValue)
        {
            hp -= hitValue;
            if (hp < 0)
            {
                hp = 0;
            }
            return hp;
        }

        private void LevelUpCannon()
        {
            cannons.LevelUp();
        }

        public void Initialize(int hp,
                                IResetEtatNotifier resetEtatNotifier)
        {
            Timer invisibilityTimer = new Timer();
            invisibilityTimer.Name = "invisibilityTimer";
            invisibilityTimer.WaitTime = 1;
            invisibilityTimer.OneShot = true;
            this.AddChild(invisibilityTimer);
            jState = new JoueurEtat(hp, invisibilityTimer);
            nouvellePosition.X = PositionCentreX();
            Position = nouvellePosition;
            resetEtatNotifier.Register(this);
            cannons.Initialize(resetEtatNotifier);
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerHealthUpdated, jState.Hp);
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerScoreUpdated, jState.Score);
        }
        public void InitialiserSize(Vector2 size)
        {
            playAeraSize = size;
        }
        public void InitialiserPosition(Vector2 position)
        {
            playAeraPosition = position;
        }
        private float PositionCentreX()
        {
            return playAeraPosition.X + playAeraSize.X * 0.5f - longueurX;
        }

        public void Disable()
        {
            Visible = false;
            area.Monitorable = false;
            area.Monitoring = false;
        }

        public void OnResetToInitaialState()
        {
            nouvellePosition.X = PositionCentreX();
            Position = nouvellePosition;
            cannons.StopReloadTimer();
        }
        public void StartTimerState()
        {

        }

#region score section
        private void ScoreUpdateListener(Enemies.EnemyObjet enemy)
        {
            UpdateScore(enemy.Enemy.Score);
            Ui.FloatingTextManager.Instance.ShowScore(enemy.GlobalPosition + new Vector2(GD.RandRange(-10, 10), GD.RandRange(-5, 5)), enemy.Enemy.Score);
        }
        private void UpdateScore(int newScore)
        {
            jState.AddScore(newScore);
            //GD.Print($"Player score = {jState.Score}");
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerScoreUpdated, jState.Score);
        }
#endregion
    }


	public interface IVie
	{
		int Hp { get; set; }
	}
	public class JoueurEtat : IVie
	{
		private int hp;
		private bool isInvicible;
		private bool isDead;
		private bool deadSoundPlayed;
		private bool endLevel;
        private int score;
        private Timer invincibilityTimer;

		public int Hp { get => hp; set => hp = value; }
		public bool IsInvicible { get => isInvicible; set => isInvicible = value; }
		public bool IsDead { get => isDead; set => isDead = value; }
		public bool DeadSoundPlayed { get => deadSoundPlayed; set => deadSoundPlayed = value; }
		public bool EndLevel { get => endLevel; set => endLevel = value; }
		public int Score { get => score; set => score = value; }
		public Timer InvincibilityTimer { get => invincibilityTimer; }


		public JoueurEtat(int hp, Timer invincibilityTimer)
		{
			this.hp = hp;
			isInvicible = false;
			isDead = false;
			deadSoundPlayed = false;
			endLevel = false;
			this.invincibilityTimer = invincibilityTimer;

            if (invincibilityTimer != null)
            {
                invincibilityTimer.Timeout += () => { isInvicible = false; };
            }
        }

        /// <summary>
		/// Add or sustract score (with negative input).
		/// </summary>
		/// <param name="newScore">score to add (>=0) or to remove (<0).</param>
        public void AddScore(int newScore) { score += newScore; }
    }
}
