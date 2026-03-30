using Godot;
using SpaceZombie.Canons;
using SpaceZombie.Events;
using SpaceZombie.Utilitaires;

namespace SpaceZombie.Joueurs
{
    /// <summary>
    /// Represents a player in the game, handling movement and shooting mechanics.
    /// </summary>
    public partial class Joueur : Area2D, IDamagable
    {
        [Export] public CanonJoueur canons;
        [Export] private Control panel;
        [Export] private AudioStreamPlayer sonDodge;
        [Export] private AudioStreamPlayer sonDodgeSimple;
        [Export] private AudioStreamPlayer sonDodgeRestore;
        [Export] private GpuParticles2D dodgeEffect;
        [Export] private ColorRect invinsibilityPanel;
        [Export] private AudioStreamPlayer sonPrendsHit;
        [Export] private AudioStreamPlayer sonMeurt;
        [Export] private AudioStreamPlayer sonInvicible;
        [Export] public int hp = 3;
        [Export] public float moveSpeed = 200f;
        [Export] private float dodgeSpeedIncrease = 350f;
        [Export] private float dodgeSimpleSpeedIncrease = 275f;
        [Export] private Timer dodgeTimer;
        [Export] private Timer dodgeDelayTimer;
        [Export] private Timer dodgeCooldownTimer;
        [Export] private DodgeEnergy dodgeEnergy1;
        [Export] private DodgeEnergy dodgeEnergy2;
        [Export] private int dodgeUpgrade = 0;
        [Export] bool godMode = false;
        public JoueurEtat jState;
        private bool isDodging = false;
        private int dodgeCount = 0;
        private float dodgeSpeed = 0;
        private Color dodgeTransparency;
        public bool IsDodging { get => isDodging; }
        private Color normalTransparency = new Color(1, 1, 1);
        private Vector2 playAeraSize;
        private Vector2 playAeraPosition;
        private Vector2 nouvellePosition;
        private float directionX = 0;
        private float demiXsize = 0;

        public override void _Ready()
        {
            playAeraPosition = Position;

            demiXsize = (int)(panel.Size.X * 0.5f);


            dodgeEffect.Emitting = false;
            dodgeTransparency = panel.Modulate;
            dodgeTransparency.A = 0.5f;
            dodgeTimer.Timeout += EndDodge;
            dodgeCooldownTimer.Timeout += EndDodgeCooldown;

            GameEvents.Instance.EnemyDied += ScoreUpdateListener;

            sonInvicible.Finished += OnSoundInvicibilityFinished;
            invinsibilityPanel.Visible = false;
            SetProcess(false);
        }

        public override void _Process(double delta)
        {
            if (dodgeSpeed == 0)
            {
                directionX = 0f;

                // Get movement direction from input
                if (Input.IsActionJustPressed("dodge") && dodgeDelayTimer.IsStopped())
                {
                    Dodge();
                }
            }

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
            Position += new Vector2(directionX * (moveSpeed + dodgeSpeed) * (float)delta, 0);

            // Clamp the position within the play area
            nouvellePosition.X = Mathf.Clamp(Position.X, playAeraPosition.X + demiXsize, playAeraPosition.X + playAeraSize.X - demiXsize);
            nouvellePosition.Y = Position.Y;

            Position = nouvellePosition;

            // Check if spacebar is pressed and reload timer is not active
            if (Input.IsActionPressed("shot_fire") && dodgeTimer.IsStopped())
            {
                canons.Fire();
            }
        }

        private void Dodge()
        {
            if (dodgeCount > dodgeUpgrade)
            {
                DoSimpleDodge();
            }
            else
            {
                DoFullDodge();
            }
            dodgeDelayTimer.Start();
            dodgeTimer.Start();
            canons.reloadTimer.Stop();
        }

        private void DoSimpleDodge()
        {
            dodgeSpeed = dodgeSimpleSpeedIncrease;
            sonDodgeSimple.Play();
        }

        private void DoFullDodge()
        {
            dodgeSpeed = dodgeSpeedIncrease;
            panel.Modulate = dodgeTransparency;
            dodgeEffect.Emitting = true;
            isDodging = true;
            dodgeCount += 1;
            sonDodge.Play();
            dodgeCooldownTimer.Start();
            if (dodgeCount > dodgeUpgrade)
            {
                dodgeEnergy1.Use();
                dodgeEnergy2.Use();
            }
        }

        private void EndDodge()
        {
            dodgeSpeed = 0;
            dodgeEffect.Emitting = false;
            panel.Modulate = normalTransparency;
            isDodging = false;
            canons.reloadTimer.Start();
        }

        private void EndDodgeCooldown()
        {
            dodgeCount = 0;
            dodgeEnergy1.Restore();
            dodgeEnergy2.Restore();
            sonDodgeRestore.Play();
        }

        public void TakeDamage(int damage)
        {
            if (!jState.IsDead && !jState.IsInvicible)
            {
                jState.Hp = RetirerHp(jState.Hp, damage);
                UpdateScore(-5000);
                GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerHealthUpdated, jState.Hp);
                if (jState.Hp > 0)
                {
                    //GD.Print("[SoundSystemJoueur] Play 'player hit' sound.");
                    jState.IsInvicible = true;
                    sonPrendsHit.Play();
                    sonInvicible.Play();
                    invinsibilityPanel.Visible = true;
                }
                else if (!jState.IsDead)
                {
                    jState.IsDead = true;
                    jState.DeadSoundPlayed = true;
                    GD.Print("[SoundSystemJoueur] Play 'player Die' sound.");
                    CallDeferred(nameof(Disable));
                    AnimationPlayer animation = GetNode<AnimationPlayer>("AnimationPlayer");
                    animation.AnimationFinished += OnDiedAnimationFinished;
                    animation.Play("Die");
                }
            }
        }

        private void OnDiedAnimationFinished(StringName animName)
        {
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.ShowEndScreen);
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

        public void Upgrade(UpgradeOptions option)
        {
            //GD.Print($"Upgrade : {option}");
            switch (option)
            {
                case UpgradeOptions.Damage: canons.UpgradeDamage(); break;
                case UpgradeOptions.AttackSpeed: canons.UpgradeVitesse(); break;
                case UpgradeOptions.AddProjectile: canons.UpgradeCanons(); break;
                case UpgradeOptions.Passthrough: canons.UpgradeTraverse(); break;
                case UpgradeOptions.Dodge: UpgradeDodge(); break;
            }
        }

        private void UpgradeDodge()
        {
            dodgeUpgrade += 1;
            if (dodgeCount == dodgeUpgrade)
            {
                dodgeEnergy1.Restore();
                dodgeEnergy2.Restore();
                sonDodgeRestore.Play();
            }
        }


        public void Initialize(Rect2 playArea)
        {
            jState = new JoueurEtat(hp);
            nouvellePosition = Position;
            nouvellePosition.X = PositionCentreX();
            Position = nouvellePosition;
            canons.Initialize();
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerHealthUpdated, jState.Hp);
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.PlayerScoreUpdated, jState.Score);
            if (godMode) SetGodMode();
            playAeraSize = playArea.Size;
            playAeraPosition = playArea.Position;
            SetProcess(true);
        }

        private void SetGodMode()
        {
            for (int i = 0; i < 8; i++)
            {
                canons.UpgradeCanons();
                canons.UpgradeVitesse();
            }
            for (int i = 0; i < 3; i++)
            {
                canons.UpgradeTraverse();
                canons.UpgradeDamage();
            }
            moveSpeed = 500f;
        }
        private float PositionCentreX()
        {
            return playAeraPosition.X + playAeraSize.X * 0.5f;
        }

        public void Disable()
        {
            SetProcess(false);
            Monitorable = false;
            Monitoring = false;
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            GameEvents.Instance.EnemyDied -= ScoreUpdateListener;
        }

        private void OnSoundInvicibilityFinished()
        {
            invinsibilityPanel.Visible = false;
            jState.IsInvicible = false;
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

        public int Hp { get => hp; set => hp = value; }
        public bool IsInvicible { get => isInvicible; set => isInvicible = value; }
        public bool IsDead { get => isDead; set => isDead = value; }
        public bool DeadSoundPlayed { get => deadSoundPlayed; set => deadSoundPlayed = value; }
        public bool EndLevel { get => endLevel; set => endLevel = value; }
        public int Score { get => score; set => score = value; }


        public JoueurEtat(int hp)
        {
            this.hp = hp;
            isInvicible = false;
            isDead = false;
            deadSoundPlayed = false;
            endLevel = false;
        }

        /// <summary>
        /// Add or sustract score (with negative input).
        /// </summary>
        /// <param name="newScore">score to add (>=0) or to remove (<0).</param>
        public void AddScore(int newScore) { score += newScore; }
    }
}
