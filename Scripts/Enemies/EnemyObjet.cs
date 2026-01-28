//EnemyObjet.cs
using Godot;
using System;

namespace SpaceZombie.Enemies
{
    public partial class EnemyObjet : Node2D
    {
        [Export] private Area2D area;
        [Export] private Panel panel;
        private Enemy enemy;

        public Enemy Enemy { get => enemy; }
        public EnemyFlagLogic enemyFlagLogic { get; private set; }
        public override void _Ready()
        {
            base._Ready();
            enemyFlagLogic = new EnemyFlagLogic();
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
            this.panel.Modulate = mapper.Color;
            this.enemy = mapper.Enemy;
        }

        // public void DisableCallDefered()
        // {
        //     area.CallDeferred(Area2D.MethodName.SetMonitorable, false);
        //     this.CallDeferred(Node2D.MethodName.SetVisible, false);
        // }
        public void Disable()
        {
            area.Monitorable = false;
            Visible = false;
        }
        // public void EnableCallDefered()
        // {
        //     area.CallDeferred(Area2D.MethodName.SetMonitorable, true);
        //     this.CallDeferred(Node2D.MethodName.SetVisible, true);
        // }
        private void Enable()
        {
            Visible = true;
            area.Monitorable = true;
        }
    }


    public class EnemyObjetMapper
    {
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public Enemy Enemy { get; set; }

        public EnemyObjetMapper(bool visible, Color color, Enemy enemy)
        {
            Visible = visible;
            Color = color;
            Enemy = enemy;
        }
    }
}
