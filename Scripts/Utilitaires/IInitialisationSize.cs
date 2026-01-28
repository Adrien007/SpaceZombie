//IInitialisationSize.cs
using Godot;

namespace SpaceZombie.Mondes.Utilitaires
{
    public interface IInitialisationSize
    {
        public void InitialiserSize(Vector2 size);
    }
    public interface IInitialisationPosition
    {
        public void InitialiserPosition(Vector2 position);
    }
}