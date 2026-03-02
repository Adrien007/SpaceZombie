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
        [Export] public float tempsRelaod = 0.5f;
        [Export] private Area2D area;

        [Export] private CanonObjet cannon0;
        private Timer reloadTimer;

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

            reloadTimer = new Timer();
            AddChild(reloadTimer);
            reloadTimer.WaitTime = tempsRelaod;
            reloadTimer.OneShot = true;
            reloadTimer.Timeout += OnReloadTimeout;

            area.AreaEntered += OnAreaEntered;
            GameEvents.Instance.LevelUp += LevelUpCannon;
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
            if (Input.IsActionPressed("shot_fire") && reloadTimer.TimeLeft == 0)
            {
                FireAllCannons(); // Fire all cannons when spacebar is pressed
            }
        }

        // Method to fire all cannons
        private void FireAllCannons()
        {
            reloadTimer.Start();
            if (cannon0.Visible)
            {
                cannon0.Fire();
            }
        }

        private void OnAreaEntered(Area2D area)
        {
            //GD.Print("Joueur Hit !!! + " + area.GetInstanceId());

            if (area.GetParent() is ProjectileObjet projectile)
            {
                if (!jState.IsDead && !jState.IsInvicible)
                {
                    jState.IsInvicible = true;
                    jState.InvincibilityTimer.Start();
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

        // Reload timer timeout handler
        private void OnReloadTimeout()
        {
            // You can add any additional logic for what happens when the reload timer finishes
            //GD.Print("Reload finished. You can fire again!");
        }


        private void LevelUpCannon()
        {
            cannon0.LevelUp();
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
            cannon0.Initialize(0, new Projectile(1, 250f, false), LayerDictionnary.GetLayer(LayerDictionnary.Enemy), resetEtatNotifier);
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
            reloadTimer.Stop();
        }
        public void StartTimerState()
        {

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
        private Timer invincibilityTimer;

        public int Hp { get => hp; set => hp = value; }
        public bool IsInvicible { get => isInvicible; set => isInvicible = value; }
        public bool IsDead { get => isDead; set => isDead = value; }
        public bool DeadSoundPlayed { get => deadSoundPlayed; set => deadSoundPlayed = value; }
        public bool EndLevel { get => endLevel; set => endLevel = value; }
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
    }
}
