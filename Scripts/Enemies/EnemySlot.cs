//EnemySlot.cs
using Godot;
using System;

namespace SpaceZombie.Enemies
{
    public partial class EnemySlot : Control
    {
        public bool EstEnemieVisible { get => enemyObj.Visible; }
        [Export] private EnemyObjet enemyObj;

        public void SetEnemyObjet(EnemyObjetMapper mapper)
        {
            enemyObj.SetEnemy(mapper);
            enemyObj.Position = this.Size * 0.5f;
        }
    }
}
