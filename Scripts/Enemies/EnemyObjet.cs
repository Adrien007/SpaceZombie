using Godot;
using SpaceZombie.Events;
using SpaceZombie.Utilitaires;

namespace SpaceZombie.Enemies
{
    public partial class EnemyObjet : Area2D, IDamagable
    {
        private Enemy enemy;
        [Export] private AudioStreamPlayer sonPrendsHit;
        [Export] private AudioStreamPlayer sonMeurt;

        public Enemy Enemy { get => enemy; }
        public EnemyFlagLogic enemyFlagLogic { get; private set; }
        public override void _Ready()
        {
            enemyFlagLogic = new EnemyFlagLogic();
        }

        public void TakeDamage(int damage)
        {
            if (!enemyFlagLogic.isDead)
            {
                Enemy.Hp = RetirerHp(Enemy.Hp, damage);
                if (Enemy.Hp <= 0)
                {
                    enemyFlagLogic.isDead = true;
                    enemyFlagLogic.deadSoundPlayed = true;
                    //GD.Print("[SoundSystemEnemy] Play 'enemy Die' sound.");
                    sonMeurt.Play(0.62f);
                    Disable();
                    GameEvents.Instance.EmitSignal(GameEvents.SignalName.EnemyDied);
                    GameEvents.Instance.EmitSignal(GameEvents.SignalName.UpdateScore, enemy.Score, GlobalPosition);

                }
                else
                {
                    //GD.Print("[SoundSystemEnemy] Play 'enemy hit' sound.");
                    sonPrendsHit.Play();
                }
            }
        }

        public void SetEnemy(EnemyObjetMapper mapper)
        {
            if (mapper.Visible)
            {
                Enable();
            }
            else
            {
                Disable();
            }
            this.enemy = mapper.Enemy;
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

        public void Disable()
        {
            Visible = false;
            SetDeferred("monitorable", false);
        }

        private void Enable()
        {
            Visible = true;
            SetDeferred("monitorable", true);
        }
    }


    public class EnemyObjetMapper
    {
        public bool Visible { get; set; }
        public PackedScene EnemyObj { get; set; }
        public Enemy Enemy { get; set; }

        public EnemyObjetMapper(bool visible, PackedScene enemyObj, Enemy enemy)
        {
            Visible = visible;
            EnemyObj = enemyObj;
            Enemy = enemy;
        }
    }
}
