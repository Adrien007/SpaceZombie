using Godot;

namespace SpaceZombie.Utilitaires
{
    public interface IDamagable
    {
        public bool IsDodging { get => false; }
        public void TakeDamage(int damage);
    }
}