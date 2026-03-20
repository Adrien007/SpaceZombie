//EnemyObjet.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Events;
using System;

namespace SpaceZombie.Enemies
{
    public partial class EnemyObjet : Node2D
    {
        [Export] private Area2D area;
        [Export] private Panel panel;
        private Enemy enemy;
        [Export] private AudioStreamPlayer sonPrendsHit;
        [Export] private AudioStreamPlayer sonMeurt;

        public Enemy Enemy { get => enemy; }
        public EnemyFlagLogic enemyFlagLogic { get; private set; }
        public override void _Ready()
        {
            base._Ready();
            area.AreaEntered += OnAreaEntered;
            enemyFlagLogic = new EnemyFlagLogic();
        }

        private void OnAreaEntered(Area2D area)
        {
            //_collisionManager.ReportCollision(this, aera2D);

            //GD.Print("Enemy Hit !!! + ");

            if (area.GetParent() is ProjectileObjet projectile && !enemyFlagLogic.isDead)
            {
                Enemy.Hp = RetirerHp(Enemy.Hp, projectile.Projectile.Damage);
                if (Enemy.Hp <= 0)
                {
                    enemyFlagLogic.isDead = true;
                    enemyFlagLogic.deadSoundPlayed = true;
                    //GD.Print("[SoundSystemEnemy] Play 'enemy Die' sound.");
                    sonMeurt.Play(0.62f);
                    Disable();
                    GameEvents.Instance.EmitSignal(GameEvents.SignalName.EnemyDied, this);
                }
                else
                {
                    //GD.Print("[SoundSystemEnemy] Play 'enemy hit' sound.");
                    sonPrendsHit.Play();
                }
                Callable.From(projectile.Disable).CallDeferred();
            }
        }
        public void SetEnemy(EnemyObjetMapper mapper)
        {
            if (mapper.Visible)
            {
                Enable();
                //EnableCallDefered();
            }
            else
            {
                Disable();
                //DisableCallDefered();
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

        private void DisableCallDefered()
        {
            area.Monitoring = false;
        }
        public void Disable()
        {
            Visible = false;
            CallDeferred(nameof(DisableCallDefered));
        }

        private void EnableCallDefered()
        {
            Visible = true;
            area.Monitoring = true;
        }
        private void Enable()
        {
            Visible = true;
            CallDeferred(nameof(EnableCallDefered));
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
