using SpaceZombie.Utilitaires;

namespace SpaceZombie.Joueurs
{
    public interface IDamagableJoueur : IDamagable
    {
        public bool canBeGrabbed { get; }
    }
}