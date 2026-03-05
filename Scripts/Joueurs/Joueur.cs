using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
using SpaceZombie.Mondes.Utilitaires;

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
        [Export] private Panel invinsibilityPanel;
        [Export] private AudioStreamPlayer sonPrendsHit;
        [Export] private AudioStreamPlayer sonMeurt;
        [Export] private AudioStreamPlayer sonInvicible;

        public JoueurEtat jState;
        private Vector2 playAeraSize;
        private Vector2 playAeraPosition;
        private int longueurX;
        private int demiLongueurX;
        private Vector2 nouvellePosition;
        private float directionX = 0;

        public override void _Ready()
        {
            base._Ready();
            longueurX = (int)enfant.Size.X;
            demiLongueurX = (int)(longueurX * 0.5f);
            playAeraSize = enfant.Size;
            playAeraPosition = Position;

            area.AreaEntered += OnAreaEntered;
            GameEvents.Instance.LevelUp += LevelUpCannon;

            sonInvicible.Finished += OnSoundInvicibilityFinished;

            invinsibilityPanel.Visible = false;
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
            //GD.Print($"{Position.X}, {playAeraPosition.X}, {playAeraPosition.X + playAeraSize.X - longueurX}");
            //nouvellePosition.X = Mathf.Clamp(Position.X, playAeraPosition.X, playAeraPosition.X + playAeraSize.X - longueurX);
            nouvellePosition.X = Mathf.Clamp(Position.X, playAeraPosition.X + demiLongueurX, playAeraPosition.X + playAeraSize.X - demiLongueurX);
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
                    sonInvicible.Play();
                    jState.Hp = RetirerHp(jState.Hp, projectile.Projectile.Damage);
                    if (jState.Hp <= 0)
                    {
                        jState.IsDead = true;
                        jState.DeadSoundPlayed = true;
                        GD.Print("[SoundSystemJoueur] Play 'player Die' sound.");
                        CallDeferred(nameof(Disable));
                        GameEvents.Instance.EmitSignal(nameof(GameEvents.PlayerDied));
                    }
                    else
                    {
                        //GD.Print("[SoundSystemJoueur] Play 'player hit' sound.");
                        sonPrendsHit.Play();
                        invinsibilityPanel.Visible = true;
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
            jState = new JoueurEtat(hp);
            nouvellePosition.X = PositionCentreX();
            Position = nouvellePosition;
            resetEtatNotifier.Register(this);
            cannons.Initialize(resetEtatNotifier);
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
            //return playAeraPosition.X + playAeraSize.X * 0.5f - longueurX;
            return playAeraPosition.X + playAeraSize.X * 0.5f - demiLongueurX;
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

        private void OnSoundInvicibilityFinished()
        {
            invinsibilityPanel.Visible = false;
            jState.IsInvicible = false;
        }
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

        public int Hp { get => hp; set => hp = value; }
        public bool IsInvicible { get => isInvicible; set => isInvicible = value; }
        public bool IsDead { get => isDead; set => isDead = value; }
        public bool DeadSoundPlayed { get => deadSoundPlayed; set => deadSoundPlayed = value; }
        public bool EndLevel { get => endLevel; set => endLevel = value; }


        public JoueurEtat(int hp)
        {
            this.hp = hp;
            isInvicible = false;
            isDead = false;
            deadSoundPlayed = false;
            endLevel = false;
        }
    }
}
